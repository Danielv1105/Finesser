using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Start()
    {

        // Die maximale Anzahl an Herzen, die HeroKnight haben kann/zugewiesen bekommt, erhält den Wert von playerHealth/HeroKnight
        // Dieser Wert kommt aus dem Skript Health und die Variable currentHealth kann durch "." zugegriffen werden
        // Das Image für die Healthbar (10 Herzen) hat den Image-Type "Filled"
        // /10 wird gerechnet, weil der "Fill-Ammount"-Wert für das Image in Prozent angegeben wird und dementsprechend ist 1 das Maximum
        // Bsp. Spieler hat 3 Herzen => currentHealth = 3 | totalhealthBar.fillAmount = 3/10 = 0.3 => 3 Herzen werden angezeigt

        totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

    // Die tatsächliche Anzahl an Herzen wird "geupdatet" und erhält den Wert von currentHealth
    // Zum Beispiel Oben: Wenn ein Herz abgezogen wird, bleibt totalhealthBar.fillAmount = 3 und man sieht die verblienden schwarzen Herzen
    // da es nur beim Start des Spiels initialisiert wird
    // in Update wird aktualisiert, wie viele Herzen der Spieler tatsächlich momentan hat
    private void Update()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

}
