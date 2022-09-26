    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FishNet.Object;

public enum BodyAnimation
{
    Idle,
    Run,
    HoldPistol,
    HoldShootgun,
    HoldRifle,
    ReloadPistol,
    ReloadRifle,
    Die
}

public enum LegsAnimation
{
    Idle,
    Run,
    Die
}

public class PlayerAnimationBehaviour : NetworkBehaviour
{
    //Références
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private Animator bodyAnimator;
    [SerializeField]
    private Animator legsAnimator;

    private PlayerLife playerLife;

    private Rigidbody2D rbd;
    
    //Get setter, change animation
    private BodyAnimation _bodyAnimation;
    public BodyAnimation bodyAnimation
    {
        get { return _bodyAnimation; }
        set
        {
            if (_bodyAnimation == value) 
                return;
            _bodyAnimation = value;
            SetBodyAnimation();
        }
        
    }

    private LegsAnimation _legsAnimation;
    public  LegsAnimation leggsAnimation
    {
        get { return _legsAnimation; }
        set
        {
            if (_legsAnimation == value)
                return;
            _legsAnimation = value;
            SetLegsAnimation();
        }
    }

    //Observer pattern (To Inverse animation)
    public delegate void StaticInverseAnimationDelegate();
    public static event StaticInverseAnimationDelegate staticInverseAnimation;


    private void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        playerLife = GetComponent<PlayerLife>();
        PlayerLife.staticRevive += ReviveConfigurationAnimation;
        PlayerLife.staticDie += DieConfigurationAnimation;

    }

    private void Update()
    {
        if (IsDie() && bodyAnimation != BodyAnimation.Die && leggsAnimation != LegsAnimation.Die)
        {
            bodyAnimation = BodyAnimation.Die;
            leggsAnimation = LegsAnimation.Die;

            return;
        }

        if (IsOwner)
        {

            if(playerData.actualPlayerWeapon != null)
            {
                switch (playerData.actualPlayerWeapon.weaponType)
                {
                    case WeaponType.Pistol:
                        if (playerData.actualPlayerWeapon.isRealoading)
                        {
                            bodyAnimation = BodyAnimation.ReloadPistol;
                        }
                        else
                        {
                            bodyAnimation = BodyAnimation.HoldPistol;
                        }
                        
                        break;
                    case WeaponType.Rifle:
                        if (playerData.actualPlayerWeapon.isRealoading)
                        {
                            bodyAnimation = BodyAnimation.ReloadRifle;
                        }
                        else
                        {
                            bodyAnimation = BodyAnimation.HoldRifle;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                bodyAnimation = BodyAnimation.Idle;
            }

            if (IsMooving())
            {
                if(playerData.actualPlayerWeapon == null)
                    bodyAnimation = BodyAnimation.Run;

                leggsAnimation = LegsAnimation.Run;

                //Manage inverse animation on legs
                legsAnimator.SetBool("Inverse", playerData.inverseAnimation);
            }
            else
            {
                leggsAnimation = LegsAnimation.Idle;
            }

        }
        

    }

    #region Player physics data

    private bool IsMooving()
    {
        if(rbd.velocity.magnitude < 0.2f)
        {
            return false;
        }

        return true;
    }

    private bool IsDie()
    {
        if(playerLife.playerState == PlayerState.Dead)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Animation controller
    private void SetBodyAnimation()
    {
        switch (bodyAnimation)
        {
            case BodyAnimation.Idle:
                SetAnimation(bodyAnimator,"WalkBody", false);
                break;
            case BodyAnimation.Run:
                SetAnimation(bodyAnimator,"WalkBody", true);
                break;
            case BodyAnimation.HoldPistol:
                SetAnimation(bodyAnimator, "HoldPistol", true);
                break;
            case BodyAnimation.ReloadPistol:
                SetAnimation(bodyAnimator, "IsReloading", true);
                bodyAnimator.SetBool("HoldPistol", true);
                break;
            case BodyAnimation.HoldShootgun:
                break;
            case BodyAnimation.HoldRifle:
                SetAnimation(bodyAnimator,"HoldRifle", true);
                break;
            case BodyAnimation.ReloadRifle:
                SetAnimation(bodyAnimator, "IsReloading", true);
                bodyAnimator.SetBool("HoldRifle", true);
                break;
            case BodyAnimation.Die:
                SetAnimation(bodyAnimator, "IsDead", true);
                break;
            default:
                break;
        }
    }

    private void SetLegsAnimation()
    {
        switch (leggsAnimation)
        {
            case LegsAnimation.Idle:
                SetAnimation(legsAnimator, "WalkLegs", false);
                break;
            case LegsAnimation.Run:
                SetAnimation(legsAnimator, "WalkLegs", true);
                break;
            case LegsAnimation.Die:
                SetAnimation(legsAnimator, "IsDead", true);
                break;
            default:
                break;
        }
    }

    void SetAnimation(Animator animator,string animation, bool onOff)
    {
        AnimatorControllerParameter param;

        for(int i = 0; i< animator.parameterCount; i++)
        {
            param = animator.parameters[i];
            if(param.type == AnimatorControllerParameterType.Bool)
            {
                if(param.name == animation)
                {
                    animator.SetBool(param.name, onOff);
                }
                else
                {
                    animator.SetBool(param.name, false);
                }
            }
        }
    }

    //Die revive
    private void ReviveConfigurationAnimation()
    {
        bodyAnimator.GetComponent<SpriteRenderer>().sortingLayerName = "default";
        legsAnimator.GetComponent<SpriteRenderer>().sortingLayerName = "default";
    }

    private void DieConfigurationAnimation()
    {
        bodyAnimator.GetComponent<SpriteRenderer>().sortingLayerName = "Dead";
        legsAnimator.GetComponent<SpriteRenderer>().sortingLayerName = "Dead";
    }
    #endregion
}
