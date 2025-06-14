﻿using DG.Tweening;
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
            SFXManager.Instance.PlayOneShot("pickup", GameParameters.Instance.PickupVolume, 1.25f, 1.25f);
            collectionParticles.GetComponent<ParticleSystem>().Play();
            collectionParticles.GetComponent<MoveParticlesToTarget>().StartMovement();
            AnalyticsManager.Instance.SendAdditionalSoapCollected();
        }

        private void HandleBlockPhase()
        {
            BubbleSpawner.Instance.CanFinishPhase = false;
        }

        private void HandleRefillSoap()
        {
            BubbleSpawner.Instance.AddSoap(GameParameters.Instance.BonusSoap, GameParameters.Instance.DurationOfSoapRefill, HandleSoapAmountChanged);
        }

        private void HandleSoapAmountChanged()
        {
            BubbleSpawner.Instance.CanFinishPhase = true;
            Destroy(collectionParticles);
            Destroy(gameObject);
        }
    }
}
