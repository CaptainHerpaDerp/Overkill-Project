using UnityEngine;

namespace Creatures.ParticleSystems
{
    public class ConversionParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem raysParticles;
        [SerializeField] private ParticleSystem sparksParticles;
        [SerializeField] private float saturationValue;

        private void OnEnable()
        {
            raysParticles.Play();
            sparksParticles.Play();
        }

        public void ActivateWithColor(Color color)
        {
            Color setColor = color;

            // Set the color's saturation to the desired value
            float h, s, v;
            Color.RGBToHSV(setColor, out h, out s, out v);
            s = saturationValue;
            setColor = Color.HSVToRGB(h, s, v);
            // Set the color
            var mainRays = raysParticles.main;
            mainRays.startColor = setColor;

            var mainSparks = sparksParticles.main;
            mainSparks.startColor = setColor;

            // Play the particle system
            this.gameObject.SetActive(true);
        }
    }
}