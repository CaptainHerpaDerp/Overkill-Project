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

        //        Plant ID, New Team Color, Old Team Color
        public Action<int, TEAMCOLOR, TEAMCOLOR> OnPlantOwnershipChanged;

        public ColorEnum.TEAMCOLOR TeamColor
        {
            get => teamColor;
            set
            {
                plantMeshFilter.mesh = plantMeshes[(int)value];

                // If the value is new, grow the plant (So that the animation doesnt trigger when the player walks on their own plants)
                if (value != teamColor)
                {
                    plantAnimator.SetTrigger("Grow");
                    SetColor(ColorEnum.GetColor(value));
                }

                // Update the plant owner in the ScoreManager
                // ScoreManager.Instance.UpdatePlantOwnership(PlantID, value, teamColor);

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