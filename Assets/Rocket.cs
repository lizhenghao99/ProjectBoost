using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 50f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisonDisabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisonDisabled = !collisonDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisonDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.volume = 0.3f;
                audioSource.PlayOneShot(success);
                mainEngineParticles.Stop();
                successParticles.Play();
                Invoke("LoadNextLevel", 1f);
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.volume = 0.3f;
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    private void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = (currentLevel + 1) % 
            SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextLevel);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // movement
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            // audio
            if (!audioSource.isPlaying)
            {
                audioSource.clip = mainEngine;
                audioSource.Play();
            }
            // particle
            mainEngineParticles.Play();

            if (audioSource.volume < 0.5f)
            {
                audioSource.volume += 0.05f;
            }
        }
        else
        {
            if (audioSource.volume == 0)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.volume -= 0.05f;
            }
            mainEngineParticles.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;

        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * -rotationSpeed);
        }

        rigidBody.freezeRotation = false;
    }
}
