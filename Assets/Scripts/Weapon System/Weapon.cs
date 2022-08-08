using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;

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
    [Header("References")]
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


    //Bullet Data
    [Header("Bullet Parameter")]
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float fireRate; //In seconds
    [SerializeField]
    private float range;
    [SerializeField]
    public int damage;

    [HideInInspector]
    public bool canShootRate = true;
    [HideInInspector]
    private double timeFireRate;


    [SerializeField]
    private GameObject impactPoint;

    [SerializeField]
    private GameObject trailImpact;
    

    //Reload data

    [SerializeField]
    protected int maxBullet;
    [SerializeField]
    protected double maxTimeToReload;

    [SyncVar]
    protected double actualTimeToReload = 0;
    [SyncVar]
    protected int actualBullet;

    [SyncVar]
    [HideInInspector] public bool isRealoading;

    //Fire Paramater
    [Header("Fire Parameter")]
    [SerializeField] private float timeToStartSpread;
    [SerializeField] private float timeToStartMaxSpread;
    [SerializeField] private float minAngleSpread;
    [SerializeField] private float maxAngleSpread;

    private double actualTimeSpread = 0;
    private float actualAngleSpread = 0;

    [Header("Weapon Sprite")]
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

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);
        actualBullet = maxBullet;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        //Client don't get good array
        //BulletsInMagazine = new GameObject[maxBullet];
    }
    private void FixedUpdate()
    {
        FireRateManager();
    }

    #region Shoot
    public virtual void Shoot()
    {
        
        if (IsOwner)
        {
            if (canShootRate)
            {
                if (actualBullet> 0 && !isRealoading)
                {
                    RaycastShoot();
                }
                else
                {
                    if (!isRealoading)
                    {
                        Reload();
                    }

                }
            }

            UpdateSpread();
        }

    }
    public virtual void RaycastShoot()
    {
        if (canShootRate)
        {
            actualBullet--;
            canShootRate = false;
            int layerMask = LayerMask.GetMask("Wall", "Player");

            Vector3 direction = Quaternion.Euler(0, Random.Range(-actualAngleSpread, actualAngleSpread), 0) * -transform.up;
            RaycastHit2D hit = Physics2D.Raycast(spawnPoint.position, direction, range, layerMask, -Mathf.Infinity, Mathf.Infinity);
            
            if (hit)
            {
                ServerRpcImpact(hit.point, transform.rotation);
                if (hit.collider.tag == "Player")
                {
                    hit.transform.GetComponent<PlayerLife>().ServerRpcTakeDamage(damage);
                }
                else
                {
                    
                }
            }
            else
            {
                ServerRpcImpact(direction * range, transform.rotation);
            }
        }
        
    }

    public virtual void UpdateSpread()
    { 
        if(actualTimeSpread < timeToStartMaxSpread)
        {
            actualTimeSpread += InstanceFinder.TimeManager.TickDelta;
        }
        if(actualTimeSpread > timeToStartSpread && actualTimeSpread < timeToStartMaxSpread)
        {
            actualAngleSpread = minAngleSpread + ((float)actualTimeSpread*((maxAngleSpread - minAngleSpread) / (timeToStartMaxSpread - timeToStartSpread )));
            Debug.Log(actualAngleSpread);
        }
        else
        {
            if (actualTimeSpread> timeToStartMaxSpread)
            {
                actualAngleSpread = maxAngleSpread;
            }
        }

    }

    public virtual void DecraseSpread()
    {
        if(actualTimeSpread > 0)
        {
            actualTimeSpread -= InstanceFinder.TimeManager.TickDelta;
            Debug.Log(actualTimeSpread);
        }
        else
        {
            actualTimeSpread = 0;
        }
    }

    [ServerRpc]
    public virtual void ServerRpcImpact(Vector3 position, Quaternion rotation)
    {
        ObserverRpcImpact(position, rotation);
    }

    [ObserversRpc]
    public virtual void ObserverRpcImpact(Vector3 position, Quaternion rotation)
    {
        GameObject trail = Instantiate(trailImpact, spawnPoint.position, transform.rotation);
        trail.GetComponent<BulletTrail>().speed = bulletSpeed;
        StartCoroutine(trail.GetComponent<BulletTrail>().TrailManager(position));
        GameObject impact = Instantiate(impactPoint, position, rotation);
        InstanceFinder.ServerManager.Spawn(impact);
    }


    #endregion

    #region Reload

    public virtual void Reload()
    {
        if(!isRealoading)
        {
            Debug.Log("Reload");
            StartCoroutine(ReloadManager(maxTimeToReload));
        }
    }

    public IEnumerator ReloadManager(double time)
    {
        isRealoading = true;
        actualTimeToReload = 0;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        while (actualTimeToReload < time)
        {
            sprite.sprite = weaponReload; //Change Sprite (reload)
            Debug.Log(actualTimeToReload);
            actualTimeToReload += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        sprite.sprite = inHandWeaponSprite;
        actualTimeToReload = 0;
        isRealoading = false;
        actualBullet = maxBullet;
        Debug.Log("End reload");
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
        gameObject.SetActive(onOff);
        GetComponent<SpriteRenderer>().enabled = onOff;
        GetComponent<SpriteRenderer>().sprite = inHandWeaponSprite;
        actualTimeToReload = 0;
        isRealoading = false;

    }


    #endregion

    #region Fire Paramater

    private void FireRateManager()
    {
        if (!canShootRate)
        {
            timeFireRate += InstanceFinder.TimeManager.TickDelta;
            if (timeFireRate > fireRate)
            {
                canShootRate = true;
                timeFireRate = 0;
            }
        }
    }




    #endregion

    #region toolBox
    private Vector3 RandomVectorMinMax(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y),0);
       
    }

    #endregion
}
