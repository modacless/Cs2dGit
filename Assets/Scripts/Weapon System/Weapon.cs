using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;
using FishNet.Component.Transforming;

public enum WeaponType
{
    Pistol,
    Rifle,
    Accessory,
    Shotgun,
}


public enum WeaponTypeInHand
{
    Primary,
    Secondary,
    Accessory,
}

[RequireComponent(typeof(SpriteRenderer),typeof(BoxCollider2D))]
public abstract class Weapon : NetworkBehaviour, IShootable, IDropable
{
    #region var
    //Références
    [Header("References")]
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    protected Transform spawnPoint;

    protected SpriteRenderer weaponSpriteRenderer;


    //And other
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private int startOrdingLayer;

    //Weapon Data
    public WeaponType weaponType;

    public WeaponTypeInHand weaponTypeInHand;

    //Observer pattern
    public delegate void StaticShootDelegate(float magnitude);
    public static event StaticShootDelegate staticShoot;

    //NetData
    [SyncVar] public Vector3 weaponGrounPosition;

    //Bullet Data
    [Header("Bullet Parameter")]
    [SerializeField]
    private float bulletSpeed; //How much time bullet travel : (range/bulletSpeed)/bulletPrecision
    [SerializeField]
    private float fireRate; //In seconds
    [SerializeField]
    private float range;
    [SerializeField]
    public int damage;
    [SerializeField]
    [Range(1,100)]
    private int bulletPrecision; // Number of raycast send
    [HideInInspector]
    private float intervalTimeBtwRay = 0;

    [HideInInspector]
    public bool canShootRate = true;
    [HideInInspector]
    private double timeFireRate;


    //Shoot
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

    //Fire Paramater
    [Header("Fire Parameter")]
    [SerializeField] private float timeToStartSpread;
    [SerializeField] private float timeToStartMaxSpread;
    [SerializeField] private float minAngleSpread;
    [SerializeField] private float maxAngleSpread;

    [SyncVar]
    [HideInInspector] public bool isRealoading;

    private double actualTimeSpread = 0;
    private float actualAngleSpread = 0;

    protected ParticleSystem shootParticle;

    [Header("Weapon Sprite")]
    //Drop
    [SerializeField]
    protected Sprite inHandWeaponSprite; //Weapon sprite in hand
    [SerializeField]
    public Sprite weaponSprite; // Weapon Sprite to throw, on ground ect...
    [SerializeField]
    protected Sprite weaponReload; //Reload sprite

    //To check if can make bounce, stun enemies ect...
    private bool isWeaponThrow = false;

    [HideInInspector]
    [SyncVar]
    public Vector3 weaponVelocity = Vector3.zero;


    [Header("Cost")]
    [SerializeField]
    public int moneyCostWeapon;

    [Header("Weapon Impact on Player")]
    [Range(0,1)] public float weightFactor;
    [SerializeField] private AnimationCurve throwCurve;

    #endregion

    private void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<SpriteRenderer>().enabled = false;
        intervalTimeBtwRay = (range / bulletSpeed) / bulletPrecision;
        shootParticle = GetComponentInChildren<ParticleSystem>();

        startOrdingLayer =  GetComponent<SpriteRenderer>().sortingOrder;
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
        UpdateThrowWeapon();
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
            StartCoroutine(RaycastShootOverTime());

        }
        
    }

    private IEnumerator RaycastShootOverTime()
    {
        int layerMask = LayerMask.GetMask("Wall", "Player");
        Vector3 direction = Quaternion.Euler(0, Random.Range(-actualAngleSpread, actualAngleSpread), 0) * -transform.up;
        Vector3 endPoint = direction * range;
        Vector3 startPosition = spawnPoint.position;

        float distance = (endPoint - startPosition).magnitude;
        float breakPointDistancePrecision = distance/bulletPrecision;

        //Trail setup

        double timeBullet = 0;
        int i = 0;

        //Play particle
        shootParticle.Play();

        ServerRpcTrailBullet(direction);

        //Observer pattern invoke
        staticShoot?.Invoke((float)actualTimeSpread);

        //Multi raycast bullet for more precision
        while (i < bulletPrecision)
        {
            timeBullet += InstanceFinder.TimeManager.TickDelta;
            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, breakPointDistancePrecision, layerMask, -Mathf.Infinity, Mathf.Infinity);
            
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
                i = bulletPrecision;
                yield break;
            }
            else
            {
                startPosition += direction * breakPointDistancePrecision;
            }
            i++;
            yield return new WaitForSecondsRealtime(intervalTimeBtwRay);
        }
        ServerRpcImpact(startPosition + (direction * range), transform.rotation);
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
        }
        else
        {
            actualTimeSpread = 0;
        }
    }

    [ServerRpc]
    public virtual void ServerRpcImpact(Vector3 position, Quaternion rotation)
    {
        GameObject impact = Instantiate(impactPoint, position, rotation);
        InstanceFinder.ServerManager.Spawn(impact);
    }

    [ServerRpc]
    public virtual void ServerRpcTrailBullet(Vector3 direction)
    {
        ObserverRpcTrailBullet(direction);
    }

    [ObserversRpc]
    public virtual void ObserverRpcTrailBullet(Vector3 direction)
    {
        GameObject trail = Instantiate(trailImpact, spawnPoint.position, transform.rotation);
        trail.GetComponent<BulletTrail>().speed = (range / bulletSpeed) * 2;
        trail.GetComponent<BulletTrail>().direction = direction;
        trail.GetComponent<BulletTrail>().time = (range / bulletSpeed);
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
            if (!isRealoading)
            {
                sprite.sprite = inHandWeaponSprite;
                actualTimeToReload = 0;
                isRealoading = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        sprite.sprite = inHandWeaponSprite;
        actualTimeToReload = 0;
        isRealoading = false;
        actualBullet = maxBullet;
        Debug.Log("End reload");
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


    #region Weapon Management

    public void HideWeapon(bool onOff)
    {
        gameObject.SetActive(onOff);
        GetComponent<SpriteRenderer>().enabled = onOff;
        GetComponent<SpriteRenderer>().sprite = inHandWeaponSprite;
        actualTimeToReload = 0;
        isRealoading = false;
    }
    /*When we just want to hide sprite weapon*/
    public void HideSpriteWeapon(bool oldValue, bool newValue, bool asServer)
    {
        GetComponent<SpriteRenderer>().enabled = newValue;
    }


    public void DropItem()
    {
        ServerRpcDropItem();

    }
    [ServerRpc]
    public void ServerRpcDropItem()
    {
        ObserverRpcDropItem();
        GetComponent<SpriteRenderer>().enabled = false;
        isRealoading = false;
        //GetComponent<NetworkTransform>().
    }

    [ObserversRpc]
    public void ObserverRpcDropItem()
    {
        GetComponent<SpriteRenderer>().sprite = weaponSprite;
        GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void PickupWeapon()
    {
        ServerRpcPickupWeapon();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerRpcPickupWeapon(NetworkConnection conn = null)
    {
        this.GiveOwnership(conn);
        ObserverRpcPickupWeapon();
        HideWeapon(false);
    }

    [ObserversRpc]
    public void ObserverRpcPickupWeapon()
    {
        GetComponent<SpriteRenderer>().sprite = inHandWeaponSprite;
        GetComponent<SpriteRenderer>().sortingOrder = startOrdingLayer;
    }

    #endregion

    #region Collision && velocity Logic
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isWeaponThrow)
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<PlayerWeaponSystem>().weaponsOnGroundNearPlayer.Add(this);
            }
        }
        else
        {

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!isWeaponThrow)
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<PlayerWeaponSystem>().weaponsOnGroundNearPlayer.Remove(this);
            }
        }
        else
        {

        }

    }

    private void UpdateThrowWeapon()
    {
        if (!isWeaponThrow && playerData.actualPlayerWeapon == this)
        {
            transform.localPosition = Vector3.zero;
        }


        //transform.position += weaponVelocity * (float)InstanceFinder.TimeManager.TickDelta;
    }

    public void ThrowWeapon(Vector3 velocityRotation)
    {
        StartCoroutine(ThrowWeaponMangager(velocityRotation));
    }

    private IEnumerator ThrowWeaponMangager(Vector3 weaponVelocity)
    {
        isWeaponThrow = true;
        Vector3 startVelocity = weaponVelocity;
        float time = 0;
        while (time < throwCurve.keys[throwCurve.length-1].time)
        {
            Debug.Log("Time : " + weaponVelocity);
            weaponVelocity = weaponVelocity.normalized * (startVelocity.magnitude * throwCurve.Evaluate(time));
            time += (float)InstanceFinder.TimeManager.TickDelta;
            transform.position += weaponVelocity * (float)InstanceFinder.TimeManager.TickDelta;
            yield return new WaitForEndOfFrame();
        }
        isWeaponThrow = false;
        yield return new WaitForEndOfFrame();
    }


    #endregion
}
