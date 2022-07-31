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
}

public enum LegsAnimation
{
    Idle,
    Run
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
    public  LegsAnimation legsAnimation
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

    private void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsOwner)
        {

            if(playerData.actualPlayerWeapon != null)
            {
                switch (playerData.actualPlayerWeapon.weaponType)
                {
                    case WeaponType.Pistol:
                        bodyAnimation = BodyAnimation.HoldPistol;
                        break;
                    case WeaponType.Rifle:
                        bodyAnimation = BodyAnimation.HoldRifle;
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

                legsAnimation = LegsAnimation.Run;
            }
            else
            {
                legsAnimation = LegsAnimation.Idle;
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
            case BodyAnimation.HoldShootgun:
                break;
            case BodyAnimation.HoldRifle:
                SetAnimation(bodyAnimator,"HoldRifle", true);
                break;
            default:
                break;
        }
    }

    private void SetLegsAnimation()
    {
        switch (legsAnimation)
        {
            case LegsAnimation.Idle:
                SetAnimation(legsAnimator, "WalkLegs", false);
                break;
            case LegsAnimation.Run:
                SetAnimation(legsAnimator, "WalkLegs", true);
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
    #endregion
}
