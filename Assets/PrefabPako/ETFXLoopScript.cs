using UnityEngine;
using System.Collections;

namespace EpicToonFX
{
	public class ETFXLoopScript : MonoBehaviour
	{
		public GameObject chosenEffect;
		public float loopTimeLimit = 2.0f;
	
		[Header("Spawn without")]
	
		public bool spawnWithoutLight = true;
		public bool spawnWithoutSound = true;

		public ParticleSystem FireCurveParticle;
		public ParticleSystem SparksParticles;
		public ParticleSystem GlowParticles;
		public ParticleSystem FireBallParticles;
		public ParticleSystem NovaParticles;

		int startPos = 15;
		bool splitParticle = false;

		void Start ()
		{	
			PlayEffect();
		}

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
				if(!splitParticle)
                {
					FireBallParticles.transform.Translate(new Vector3(startPos, 0, 0));
					SparksParticles.transform.Translate(new Vector3(startPos * 2, 0, 0));
					GlowParticles.transform.Translate(new Vector3(startPos * 3, 0, 0));
					FireBallParticles.transform.Translate(new Vector3(startPos * 4, 0, 0));
					NovaParticles.transform.Translate(new Vector3(startPos * 5, 0, 0));
				}
				else
                {
					FireBallParticles.transform.Translate(Vector3.zero);
					SparksParticles.transform.Translate(Vector3.zero);
					GlowParticles.transform.Translate(Vector3.zero);
					FireBallParticles.transform.Translate(Vector3.zero);
					NovaParticles.transform.Translate(Vector3.zero);
				}
				splitParticle = !splitParticle;
            }
        }

        public void PlayEffect()
		{
			StartCoroutine("EffectLoop");
		}

		IEnumerator EffectLoop()
		{
			GameObject effectPlayer = (GameObject) Instantiate(chosenEffect, transform.position, transform.rotation);
		
			if(spawnWithoutLight = true && effectPlayer.GetComponent<Light>())
			{
				effectPlayer.GetComponent<Light>().enabled = false;
				//Destroy(gameObject.GetComponent<Light>());

			}
		
			if(spawnWithoutSound = true && effectPlayer.GetComponent<AudioSource>())
			{
				effectPlayer.GetComponent<AudioSource>().enabled = false;
				//Destroy(gameObject.GetComponent<AudioSource>());
			}
				
			yield return new WaitForSeconds(loopTimeLimit);

			Destroy (effectPlayer);
			PlayEffect();
		}
	}
}