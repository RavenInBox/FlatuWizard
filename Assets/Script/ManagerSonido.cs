using System;
using UnityEngine;

public class ManagerSonido : MonoBehaviour
{
    [Serializable]
    private class Pistas
    {
        public string Nombre;
        public AudioClip Pista;

        [Range(0f, 1f)]
        public float Volumen;
        [Range(.1f, 3f)]
        public float Velocidad;

        public bool Repetir;
        [HideInInspector]
        public AudioSource Fuente;
    }

    [SerializeField] private Pistas[] pistas;

    protected void Awake()
    {
        foreach (Pistas s in pistas)
        {
            s.Fuente = gameObject.
            AddComponent<AudioSource>();
            s.Fuente.clip = s.Pista;
            s.Fuente.volume = s.Volumen;
            s.Fuente.pitch = s.Velocidad;
            s.Fuente.loop = s.Repetir;
        }
    }

    public void Play(string nombre)
    {
        Pistas s = Array.Find(pistas, sonido => sonido.Nombre == nombre);
        if (s == null) return;
        s.Fuente.Play();
    }
}
