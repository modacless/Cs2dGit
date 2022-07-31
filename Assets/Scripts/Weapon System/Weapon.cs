using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public enum WeaponType
{
    Pistol,
    Rifle,
}

public enum WeaponState
{
    InHand,
    OnGround,
    Reload
}

public enum WeaponTypeInHand
{
    Primary,
    Secondary,
    Accessory
}

public abstract class Weapon : NetworkBehaviour, IShootable
{
    #region var
    //Références

    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    protected GameObject bulletInstance;
    [SerializeField]
    protected Transform spawnPoint;

    protected SpriteRenderer weaponSpriteRenderer;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    //Weapon Data
    public WeaponType weaponType;
    public WeaponState weaponState
    {
        get { return weaponState; }
        set 
        {
            weaponState = value;
            ChangeState();
        }
    }

    public WeaponTypeInHand weaponTypeInHand;

    //Reload data

    [SerializeField]
    protected int maxBullet;
    [SerializeField]
    protected double maxTimeToReload;

    protected double actualTimeToReload;
    protected int actualBullet;

    protected bool isRealoading;

    //Drop
    [SerializeField]
    protected Sprite inHandWeaponSprite;
    [SerializeField]
    protected Sprite weaponSprite;
    [SerializeField]
    protected Sprite weaponReload;

    #endregion

    private void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    #region Shoot
    public virtual void Shoot()
    {
        if (IsOwner)
        {
            if(actualBullet > 0)
            {
                ClientRpcShoot();
            }
            else
            {
                if (!isRealoading)
                {
                    ClientRpcReload(actualTimeToReload);
                }

            }

        }

    }
    [ServerRpc(RequireOwnership = true)]
    public virtual void ServerRpcShoot()
    {
        ClientRpcShoot();
    }
    [ObserversRpc(IncludeOwner = true)]
    public virtual void ClientRpcShoot()
    {
        actualBullet--;
        InstantiateBullet();
    }

    public virtual void InstantiateBullet()
    {
        GameObject bullet = Instantiate(bulletInstance, spawnPoint.position, transform.rotation);
    }

    #endregion

    #region Reload

    public virtual void Reload()
    {
        if(!isRealoading && Input.GetKeyDown(playerData.reloadKey))
        {
            ServerRpcReload();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void ServerRpcReload()
    {
        ClientRpcReload(actualTimeToReload);
    }
    [ObserversRpc(IncludeOwner = true)]
    public void ClientRpcReload(double time)
    {
        StartCoroutine(ReloadManager(time));
    }

    protected virtual IEnumerator ReloadManager(double time)
    {
        isRealoading = true;
        while (actualTimeToReload < time)
        {
            if(weaponSpriteRenderer.sprite != weaponReload)
            {
                actualTimeToReload += InstanceFinder.TimeManager.TickDelta;
                Debug.Log(actualTimeToReload);
                yield return waitForFixedUpdate;
            }
            else
            {
                actualTimeToReload = 0;
                yield return null;
            }
            
        }

        isRealoading = false;
    }

    private void ChangeState()
    {
        switch (weaponState)
        {
            case WeaponState.InHand:
                weaponSpriteRenderer.sprite = inHandWeaponSprite;
                break;
            case WeaponState.OnGround:
                weaponSpriteRenderer.sprite = weaponSprite;
                break;
            case WeaponState.Reload:
                weaponSpriteRenderer.sprite = weaponReload;
                break;
        }
    }

    public void HideWeapon(bool onOff)
    {
        Debug.Log("Hide + " + onOff);
        gameObject.SetActive(onOff);
        GetComponent<SpriteRenderer>().enabled = onOff;
    }

    #endregion
}
