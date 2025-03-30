using DG.Tweening;
using UnityEngine;

namespace Assets._Scripts
{
    public class Soap : MonoBehaviour
    {
        public GameObject idleParticleEffect;
        public GameObject collectionParticles;
        
        private void OnEnable()
        {
            collectionParticles.GetComponent<MoveParticlesToTarget>().OnParticlesMovementStart += HandleBlockPhase;
            collectionParticles.GetComponent<MoveParticlesToTarget>().OnFirstParticleReachedTarget += HandleRefillSoap;
        }

        private void OnDisable()
        {
            collectionParticles.GetComponent<MoveParticlesToTarget>().OnParticlesMovementStart -= HandleBlockPhase;
            collectionParticles.GetComponent<MoveParticlesToTarget>().OnFirstParticleReachedTarget -= HandleRefillSoap;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Bubble")) return;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(idleParticleEffect);
            SFXManager.Instance.PlaySound(SFXManager.Instance.PickupSound, 1.25f, 1.25f, GameParameters.Instance.PickupVolume);
            collectionParticles.GetComponent<ParticleSystem>().Play();
            collectionParticles.GetComponent<MoveParticlesToTarget>().StartMovement();
        }

        private void HandleBlockPhase()
        {
            BubbleSpawner.Instance.CanFinishPhase = false;
        }

        private void HandleRefillSoap()
        {
            BubbleSpawner.Instance.AddSoap(GameParameters.Instance.BonusSoap, HandleSoapAmountChanged);
        }

        private void HandleSoapAmountChanged()
        {
            BubbleSpawner.Instance.CanFinishPhase = true;
            Destroy(collectionParticles);
            Destroy(gameObject);
        }
    }
}
