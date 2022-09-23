using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerRotation : NetworkBehaviour
{
    //Références
    [Header("References")]
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private Camera cameraPlayer;
    [SerializeField]
    private GameObject bodyObject;
    [SerializeField]
    private GameObject legsObject;
    [SerializeField]
    private GameObject legsObjectPivot; //Will rotate this gameobject to add Keys rotation

    private PlayerLife playerLife;

    void Start()
    {
        playerLife = GetComponent<PlayerLife>();
    }

    void Update()
    {
        if (IsOwner && playerLife.playerState != PlayerState.Dead)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            bodyObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + 90f);
            LeggsRotationManager();
        }

        if(playerLife.playerState == PlayerState.Dead)
        {
            legsObject.transform.rotation = bodyObject.transform.rotation;

        }

    }
     
    void LeggsRotationManager()
    {
        Vector3 rotationVector = bodyObject.transform.rotation.eulerAngles;

        int angleRotation = 40;

        int wichAngleToTake = (int)Mathf.Round(rotationVector.z/ angleRotation);
        float rotationPerInput = LegsPivotRotation(wichAngleToTake);
        
        if (wichAngleToTake == 0 || wichAngleToTake == 9)
        {
            legsObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            legsObjectPivot.transform.rotation = Quaternion.Euler(0, 0, rotationPerInput);
        }
        else
        {
            if (wichAngleToTake == 5 || wichAngleToTake == 4)
            {
                legsObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
                legsObjectPivot.transform.rotation = Quaternion.Euler(0, 0, rotationPerInput);
            }
            else
            {
                
                legsObjectPivot.transform.rotation = Quaternion.Euler(0, 0, rotationPerInput);
                if (rotationPerInput == 0)
                {
                    legsObject.transform.localRotation = Quaternion.Euler(0, 0, wichAngleToTake * angleRotation);
                }
                else
                {
                    legsObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }

    private float LegsPivotRotation(int angleBase)
    {
        Vector2 inputDirection = InputDirection();
        playerData.inverseAnimation = false;

        if (inputDirection == Vector2.zero)
        {
            return 0;
        }

        if (angleBase == 0 || angleBase == 9)
        {
            if (inputDirection == Vector2.zero)
            {
                return 0;
            }

            if (inputDirection.y == 0)
            {
                if (inputDirection.x == 1) // right
                {
                    return 75;
                }

                if (inputDirection.x == -1)// left
                {
                    return -75;
                }
            }


            if (inputDirection.x == 0)
            {
                if (inputDirection.y == 1)// up
                {
                    playerData.inverseAnimation = true;
                    return 0;
                }

                if (inputDirection.y == -1)// down
                {
                    return 0;
                }
            }

            if(inputDirection.x == 1 && inputDirection.y == 1) // up right
            {
                playerData.inverseAnimation = true;
                return -45;
            }


            if (inputDirection.x == -1 && inputDirection.y == 1) // up left
            {
                playerData.inverseAnimation = true;
                return 45;
            }


            if (inputDirection.x == 1 && inputDirection.y == -1) // down right
            {
                return 45;
            }

            if (inputDirection.x == -1 && inputDirection.y == -1) // down left
            {
                return -45;
            }

        }

        if(angleBase == 5 || angleBase == 4)
        {

            if (inputDirection.y == 0)
            {
                if (inputDirection.x == 1) // right
                {
                    return -75;
                }

                if (inputDirection.x == -1)// left
                {
                    return 75;
                }
            }


            if (inputDirection.x == 0)
            {
                if (inputDirection.y == 1)// up
                {
                    return 0;
                }

                if (inputDirection.y == -1)// down
                {
                    return 0;
                }
            }


            if (inputDirection.x == 1 && inputDirection.y == 1) // up right
            {
                return -45;
            }


            if (inputDirection.x == -1 && inputDirection.y == 1) // up left
            {
                return 45;
            }


            if (inputDirection.x == 1 && inputDirection.y == -1) // down right
            {
                playerData.inverseAnimation = true;
                return 45;
            }


            if (inputDirection.x == -1 && inputDirection.y == -1) // down left
            {
                playerData.inverseAnimation = true;
                return -45;
            }
        }

        if (angleBase == 1 || angleBase == 2 || angleBase == 3)
        {

            if (inputDirection.y == 0)
            {
                if (inputDirection.x == 1) // right
                {
                    return 90;
                }

                if (inputDirection.x == -1)// left
                {
                    return 90;
                }
            }


            if (inputDirection.x == 0)
            {
                if (inputDirection.y == 1)// up
                {
                    if(angleBase == 1)
                    {
                        playerData.inverseAnimation = true;
                        return 360;
                    }
                    else
                    {
                        return 155;
                    }
                    
                }

                if (inputDirection.y == -1)// down
                {
                    if (angleBase == 3)
                    {
                        playerData.inverseAnimation = true;
                        return 180;
                    }
                    else
                    {
                        return 25;
                    }
                    
                }
            }

            if (inputDirection.x == 1 && inputDirection.y == 1) // up right
            {
                return 135;
            }


            if (inputDirection.x == -1 && inputDirection.y == 1) // up left
            {
                playerData.inverseAnimation = true;
                return 45;
            }


            if (inputDirection.x == 1 && inputDirection.y == -1) // down right
            {
                return 45;
            }


            if (inputDirection.x == -1 && inputDirection.y == -1) // down left
            {
                playerData.inverseAnimation = true;
                return 135;
            }



        }

        if (angleBase == 6 || angleBase == 7 || angleBase == 8)
        {

            if (inputDirection.y == 0)
            {
                if (inputDirection.x == 1) // right
                {
                    return -90;
                }

                if (inputDirection.x == -1)// left
                {
                    return -90;
                }
            }


            if (inputDirection.x == 0)
            {

                if (inputDirection.y == 1)// up
                {
                    if (angleBase == 8)
                    {
                        playerData.inverseAnimation = true;
                        return 360;
                    }
                    else
                    {
                        return -155;
                    }
                }

                if (inputDirection.y == -1)// down
                {
                    if (angleBase == 6)
                    {
                        playerData.inverseAnimation = true;
                        return 360;
                    }
                    else
                    {
                        return -25;
                    }
                }
            }

            if (inputDirection.x == 1 && inputDirection.y == 1) // up right
            {
                playerData.inverseAnimation = true;
                return -45;
            }


            if (inputDirection.x == -1 && inputDirection.y == 1) // up left
            {
                return -135;
            }


            if (inputDirection.x == 1 && inputDirection.y == -1) // down right
            {
                playerData.inverseAnimation = true;
                return -135;
            }


            if (inputDirection.x == -1 && inputDirection.y == -1) // down left
            {
                return -45;
            }
        }

        return 0;
    }

    private Vector2 InputDirection()
    {
        int left = Input.GetKey(playerData.leftKey) ? 1 : 0;
        int right = Input.GetKey(playerData.rightKey) ? 1 : 0;
        int down = Input.GetKey(playerData.downKey) ? 1 : 0;
        int up = Input.GetKey(playerData.upKey) ? 1 : 0;

        return new Vector2(right - left, up - down);
    }

}
