using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Weapon : NetworkBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private PlayerStatus playerStatus;

    [Header("Position")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform modelAnchor;
    [SerializeField] private Transform muzzlePoint;

    [Header("Effect")]
    [SerializeField] private ParticleSystem muzzleFire;
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private AudioSource reloadSound;

    [Header("Ammo")]
    public NetworkVariable<int> currentAmmo =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> getCurrentAmmo()
    {
        return currentAmmo;
    }

    [Header("ReloadTime")]
    [SerializeField] private float reloadTime;
    private float reloadTimer = 0.0f;

    private GameObject currentModel;
    private ParticleSystem muzzleFireInstance;
    private Coroutine muzzleCoroutine;

    public Transform FirePoint => firePoint;
    public WeaponData WeaponData => weaponData;
    public WeaponData Data => weaponData;

    public float Cooldown
    {
        get
        {
            if (weaponData == null)
                return 0f;

            return weaponData.cooldown;
        }
    }

    private void Awake()
    {
    }

    public void SetUp(
        PlayerStatus playerStatus,
        Transform firePoint,
        WeaponData weaponData)
    {
        this.playerStatus = playerStatus;
        this.firePoint = firePoint;
        this.weaponData = weaponData;

        // 弾薬変更はServerのみ
        if (IsServer)
        {
            ResetAmmo();
        }

        RefreshModel();
        CreateMuzzleEffect();
    }

    private void ResetAmmo()
    {
        if (!IsServer)
        {
            return;
        }

        if (weaponData == null)
        {
            return;
        }

        if (weaponData.attackType == AttackType.Gun)
        {
            currentAmmo.Value =
                weaponData.maxAmmo;
        }
        else
        {
            currentAmmo.Value = 0;
        }

        reloadTime =
            weaponData.reloadTime;
    }

    private void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        // リロード管理はServerだけ
        if (!IsServer)
        {
            return;
        }

        if (weaponData == null)
        {
            return;
        }

        if (currentAmmo.Value <= 0)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                currentAmmo.Value =
                    weaponData.maxAmmo;

                reloadTimer = 0.0f;

                PlayReloadSoundClientRpc();

                Debug.Log("リロード完了");
            }
        }
        else
        {
            reloadTimer = 0.0f;
        }
    }

    private void RefreshModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        if (weaponData == null)
            return;

        if (weaponData.GunPrefab == null)
            return;

        if (modelAnchor == null)
            return;

        currentModel = Instantiate(
            weaponData.GunPrefab,
            modelAnchor
        );

        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
    }

    private void CreateMuzzleEffect()
    {
        if (muzzleFireInstance != null)
        {
            Destroy(muzzleFireInstance.gameObject);
            muzzleFireInstance = null;
        }

        if (muzzleFire == null)
        {
            Debug.LogWarning("MuzzleFireが設定されていません");
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogWarning("MuzzlePointが設定されていません");
            return;
        }

        muzzleFireInstance = Instantiate(
            muzzleFire,
            muzzlePoint
        );

        muzzleFireInstance.transform.localPosition = Vector3.zero;
        muzzleFireInstance.transform.localRotation = Quaternion.identity;

        ParticleSystem[] particles =
            muzzleFireInstance.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule main = particle.main;

            main.loop = false;
            main.playOnAwake = false;

            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        Debug.Log("マズルフラッシュ生成完了");
    }

    public void Attack()
    {
        if (weaponData == null)
            return;

        switch (weaponData.attackType)
        {
            case AttackType.Unarmed:
            case AttackType.Melee:
                DoMeleeAttack();
                break;

            case AttackType.Gun:
                DoGunAttack();
                break;
        }
    }

    private void DoMeleeAttack()
    {
        Debug.Log($"{weaponData.attackName} で近接攻撃");
    }

    private void DoGunAttack()
    {
        if (currentAmmo.Value <= 0)
        {
            Debug.Log("弾切れ");
            return;
        }

        if (playerStatus == null)
        {
            Debug.LogWarning("PlayerStatusが設定されていません");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("FirePointが設定されていません");
            return;
        }

        Vector3 fireDirection =
            playerStatus.transform.forward.normalized;

        Vector3 spawnPosition =
            firePoint.position +
            fireDirection * 0.9f;

        FireBulletServerRpc(
            spawnPosition,
            fireDirection
        );
    }

    [ServerRpc]
    private void FireBulletServerRpc(
    Vector3 spawnPosition,
    Vector3 fireDirection)
    {
        if (weaponData == null)
        {
            return;
        }

        if (weaponData.bulletPrefab == null)
        {
            return;
        }

        if (playerStatus == null)
        {
            return;
        }

        // ============================
        // Server側で攻撃可能か確認
        // ============================

        if (!playerStatus.canShoot.Value)
        {
            return;
        }

        // Server側で弾数確認
        if (currentAmmo.Value <= 0)
        {
            Debug.Log("弾切れ");

            return;
        }

        // ★ Serverだけが弾を減らす
        currentAmmo.Value--;

        Debug.Log(
            $"残弾数 = {currentAmmo.Value}"
        );

        Quaternion rotation =
            Quaternion.LookRotation(
                fireDirection
            );

        GameObject bullet =
            Instantiate(
                weaponData.bulletPrefab,
                spawnPosition,
                rotation
            );

        BulletMove bulletMove =
            bullet.GetComponent<BulletMove>();

        NetworkObject networkObject =
            bullet.GetComponent<NetworkObject>();

        if (bulletMove == null ||
            networkObject == null)
        {
            Destroy(bullet);

            return;
        }

        bulletMove.InitializeOnServer(
            weaponData,
            playerStatus.playerID.Value,
            fireDirection
        );

        networkObject.Spawn();

        PlayFireEffectClientRpc();
    }

    [ClientRpc]
    private void PlayFireEffectClientRpc()
    {
        PlayFireEffect();
    }

    private void PlayFireEffect()
    {
        // =========================
        // マズルフラッシュ
        // =========================
        if (muzzleFireInstance != null)
        {
            ParticleSystem[] particles =
                muzzleFireInstance.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem particle in particles)
            {
                particle.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );

                particle.Play();
            }

            // Asset内のLight
            WFX_LightFlicker[] lights =
                muzzleFireInstance.GetComponentsInChildren<WFX_LightFlicker>(true);

            foreach (WFX_LightFlicker light in lights)
            {
                light.Flash();
            }

            if (muzzleCoroutine != null)
            {
                StopCoroutine(muzzleCoroutine);
            }

            muzzleCoroutine = StartCoroutine(StopMuzzleFlash());
        }

        // =========================
        // 発砲音
        // =========================
        if (fireSound == null)
        {
            Debug.LogWarning("fireSound が設定されていません");
            return;
        }

        if (fireSound.clip == null)
        {
            Debug.LogWarning("AudioClip が設定されていません");
            return;
        }

        // テストのため確実に聞こえる設定にする
        fireSound.mute = false;
        fireSound.volume = 1.0f;
        fireSound.pitch = 1.0f;

        // 完全な2D音声
        // カメラとの距離による音量減衰を無効化
        fireSound.spatialBlend = 0.0f;

        Debug.Log(
            $"発砲音再生 Clip={fireSound.clip.name} " +
            $"Volume={fireSound.volume} " +
            $"Object={fireSound.gameObject.name}"
        );

        fireSound.PlayOneShot(
            fireSound.clip,
            0.8f
        );
    }

    [ClientRpc]
    private void PlayReloadSoundClientRpc()
    {
        if (reloadSound == null ||
            reloadSound.clip == null)
        {
            return;
        }

        reloadSound.PlayOneShot(
            reloadSound.clip,
            1.0f
        );
    }

    private IEnumerator StopMuzzleFlash()
    {
        yield return new WaitForSeconds(0.08f);

        if (muzzleFireInstance != null)
        {
            ParticleSystem[] particles =
                muzzleFireInstance.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem particle in particles)
            {
                particle.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }

        muzzleCoroutine = null;
    }
}