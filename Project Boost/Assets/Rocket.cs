using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 500f;
	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip death;
	[SerializeField] AudioClip levelComplete;
	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem successParticles;
	[SerializeField] ParticleSystem deathParticles;
	[SerializeField] float levelLoadDelay = 2f;

	bool isTransitioning = false;
	bool debug = false;
	Rigidbody rigidbody;
    AudioSource audioSource;

	void Start ()
    {
        rigidbody = GetComponent<Rigidbody> ();
        audioSource = GetComponent<AudioSource> ();
    }
	
	void Update ()
    {
		if ( !isTransitioning )
		{
			RespondToThrustInput ();
			RespondToRotateInput ();
		}

		if ( Debug.isDebugBuild )
		{
			RespondToDebugInput ();
		}
	}

	private void RespondToDebugInput ()
	{
		if ( Input.GetKeyDown ( KeyCode.L ) )
		{
			LoadNextLevel ();
		}

		if ( Input.GetKeyDown ( KeyCode.C ) )
		{
			debug = !debug;
		}
	}

	private void OnCollisionEnter(Collision collision)
    {
		if ( isTransitioning || debug ) { return; }

        switch ( collision.gameObject.tag )
        {
            case "Friendly":
                break;
			case "Finish":
				StartSuccessSequence ();
				break;
			default:
				StartDeathSequence ();
				break;

		}
    }

	private void StartSuccessSequence ()
	{
		isTransitioning = true;
		audioSource.Stop ();
		successParticles.Play ();
		audioSource.PlayOneShot ( levelComplete );
		Invoke ( "LoadNextLevel", levelLoadDelay );
	}

	private void StartDeathSequence ()
	{
		isTransitioning = true;
		audioSource.Stop ();
		deathParticles.Play ();
		audioSource.PlayOneShot ( death );
		Invoke ( "LoadFirstLevel", levelLoadDelay );
	}

	private void LoadNextLevel ()
	{
		int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
		int nextSceneIndex = currentSceneIndex + 1;
		if (nextSceneIndex < SceneManager.sceneCountInBuildSettings )
		{
			SceneManager.LoadScene ( nextSceneIndex );
		}
		else
		{
			LoadFirstLevel ();
		}
	}

	private void LoadFirstLevel ()
	{
		SceneManager.LoadScene ( 0 );
	}

	private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust ();
		}
		else
		{
			StopApplyingThrust ();
		}
	}

	private void StopApplyingThrust ()
	{
		audioSource.Stop ();
		mainEngineParticles.Stop ();
	}

	private void ApplyThrust ()
	{
		rigidbody.AddRelativeForce ( Vector3.up * mainThrust * Time.deltaTime );

		if ( !audioSource.isPlaying )
		{
			audioSource.PlayOneShot ( mainEngine );
		}
		mainEngineParticles.Play ();
	}

	private void RespondToRotateInput()
    {
		rigidbody.angularVelocity = Vector3.zero; // remove rotation due to physics

		float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidbody.freezeRotation = false;
    }
}