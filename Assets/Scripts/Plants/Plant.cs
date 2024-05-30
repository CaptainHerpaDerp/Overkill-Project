using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamColors;
using System;
using static TeamColors.ColorEnum;

namespace GaiaElements
{
    public class Plant : MonoBehaviour
    {
        public Renderer plantRenderer;

        [SerializeField] private MeshFilter plantMeshFilter;

        [SerializeField] private List<Mesh> plantMeshes = new();

        [SerializeField] private ColorEnum.TEAMCOLOR teamColor;

        [SerializeField] private Animator plantAnimator;

        public int PlantID { get; private set; }

        //Plant ID, New Team Color, Old Team Color
        public Action<int, TEAMCOLOR, TEAMCOLOR> OnPlantOwnershipChanged;
        public Action OnPlantSettingsChanged;

        private bool plantSpreadCreep;

        public Transform PlayerParentTransform;

        // Ensure that the plant is not being converted by multiple players
        private Coroutine shiftColourRoutine;

        public bool PlantSpreadCreep
        {
            get { return plantSpreadCreep; }
            set
            {
                plantSpreadCreep = value;
                OnPlantSettingsChanged?.Invoke();
            }
        }

        public ColorEnum.TEAMCOLOR TeamColor
        {
            get => teamColor;
            set
            {
                if (plantMeshes.Count >= (int)value )
                plantMeshFilter.mesh = plantMeshes[(int)value];

                //Stop any current animations
                plantAnimator.enabled = true;   

                plantAnimator.StopPlayback();

                if (teamColor == TEAMCOLOR.DEFAULT)
                {
                    plantRenderer.enabled = true;
                }

                // If the value is new, grow the plant (So that the animation doesnt trigger when the player walks on their own plants)
                if (value != teamColor)
                {
                    plantAnimator.SetTrigger("Grow");
                    SetColor(ColorEnum.GetColor(value));
                }

                plantSpreadCreep = false;

                OnPlantOwnershipChanged?.Invoke(PlantID, value, teamColor);
     
                teamColor = value;
            }
        }


        private const float SHIFT_SPEED = 0.1f;

        private void Awake()
        {
            PlantID = gameObject.GetInstanceID();
        }

        private void Start()
        {
            SetColorInstant(Color.white);
            plantRenderer.enabled = false;
        }

        public bool IsOnSurface()
        {
            return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, layerMask: 6);
        }

        public void Activate(ColorEnum.TEAMCOLOR PlayerNumber)
        {
            plantRenderer.enabled = true;
            TeamColor = PlayerNumber;
            SetColor(ColorEnum.GetColor(teamColor));
        }

        /// <summary>
        /// Returns the plant to its default state
        /// </summary>
        public void UnPlant()
        {
            //TODO: There is a bug where the plant is not un-planted yet remains black. This probably happens when another player converts it before the animaton is complete

            // Do not unplant if the plant is already un-planted
            if (teamColor == TEAMCOLOR.DEFAULT)
            {
                return;
            }
            
            plantAnimator.SetTrigger("UnGrow");
            TeamColor = TEAMCOLOR.DEFAULT;

            StartCoroutine(HidePlant(1));
        }

        private IEnumerator HidePlant(float time)
        {
            yield return new WaitForSeconds(time);
            plantRenderer.enabled = false;
        }

        public void SetColorInstant(Color color)
        {
            plantRenderer.material.color = color;
        }

        public void SetColor(Color color)
        {
            StopAllCoroutines();
            StartCoroutine(ShiftColor(color));
        }

        /// <summary>
        /// Change the color of the plant over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShiftColor(Color newColor)
        {
            while (true)
            {
                // lerp the color of the plant
                plantRenderer.material.color = Color.Lerp(plantRenderer.material.color, newColor, SHIFT_SPEED);

                yield return new WaitForFixedUpdate();

                if (plantRenderer.material.color == newColor)
                {
                    break;
                }
            }
        }
    }
}