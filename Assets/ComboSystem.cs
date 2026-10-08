using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class ComboSystem : MonoBehaviour
{
    [Header("Secuencias de Combos")]
    public string comboDerecha = "UUDDQA";
    public string comboIzquierda = "UUU"; 
    public string comboArriba = "UUUDQA";  

    [Header("Fuerzas de Salto")]
    public float fuerzaSalto = 7f;  
    public float fuerzaLateral = 4f; 

    [Header("Sonidos Únicos")]
    public AudioClip sonidoDerecha;
    public AudioClip sonidoArriba;
    public AudioClip sonidoIzquierda;

    private Rigidbody rb;
    private AudioSource audioSource;

    private string historialTeclas = "";
    private float tiempoDesdeUltimaTecla = 0f;
    public float tiempoMaximoEntreTeclas = 0.35f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (historialTeclas.Length > 0)
        {
            tiempoDesdeUltimaTecla += Time.deltaTime;

            if (tiempoDesdeUltimaTecla > tiempoMaximoEntreTeclas)
            {
                EjecutarComboPendienteOReiniciar();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) RegistrarTecla("U");
        if (Input.GetKeyDown(KeyCode.DownArrow)) RegistrarTecla("D");
        if (Input.GetKeyDown(KeyCode.Q)) RegistrarTecla("Q");
        if (Input.GetKeyDown(KeyCode.A)) RegistrarTecla("A");
    }

    void RegistrarTecla(string tecla)
    {
        historialTeclas += tecla;
        tiempoDesdeUltimaTecla = 0f; 

        if (historialTeclas.Length > 10)
        {
            historialTeclas = historialTeclas.Substring(historialTeclas.Length - 10);
        }

        RevisarCombos();
    }

    void RevisarCombos()
    {
        string comboCompletado = ObtenerComboCoincidente();

        if (!string.IsNullOrEmpty(comboCompletado))
        {
            if (EsPrefijoDeComboMasLargo(historialTeclas))
            {
                return;
            }
            EjecutarCombo(comboCompletado);
        }
    }

    string ObtenerComboCoincidente()
    {
        if (historialTeclas.EndsWith(comboArriba)) return comboArriba;
        if (historialTeclas.EndsWith(comboDerecha)) return comboDerecha;
        if (historialTeclas.EndsWith(comboIzquierda)) return comboIzquierda;
        return null;
    }

    bool EsPrefijoDeComboMasLargo(string secuencia)
    {
        string[] combos = { comboDerecha, comboArriba, comboIzquierda };
        foreach (string combo in combos)
        {
            if (combo.Length > secuencia.Length && combo.StartsWith(secuencia))
            {
                return true;
            }
        }
        return false;
    }

    void EjecutarComboPendienteOReiniciar()
    {
        string comboCompletado = ObtenerComboCoincidente();
        if (!string.IsNullOrEmpty(comboCompletado))
        {
            EjecutarCombo(comboCompletado);
        }
        else
        {
            historialTeclas = "";
        }
    }

    void EjecutarCombo(string combo)
    {
        if (combo == comboDerecha)
        {
            EjecutarSalto(new Vector3(fuerzaLateral, fuerzaSalto, 0), sonidoDerecha);
        }
        else if (combo == comboArriba)
        {
            EjecutarSalto(new Vector3(0, fuerzaSalto, 0), sonidoArriba);
        }
        else if (combo == comboIzquierda)
        {
            EjecutarSalto(new Vector3(-fuerzaLateral, fuerzaSalto, 0), sonidoIzquierda);
        }

        historialTeclas = "";
    }

    void EjecutarSalto(Vector3 direccionFuerza, AudioClip sonido)
    {
        rb.linearVelocity = Vector3.zero; 
        rb.AddForce(direccionFuerza, ForceMode.Impulse);

        if (sonido != null)
        {
            audioSource.PlayOneShot(sonido);
        }
    }
}