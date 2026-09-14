using System;

namespace AlternativeCallouts.Core
{
    public enum EAlternativeOutcome
    {
        PeacefulCompliance = 0,     // Alternativa 1: Cooperacion / Resolucion pacifica / Mediacion
        ResistAndFight = 1,          // Alternativa 2: Resistencia activa / Forcejeo / Arma blanca
        FootPursuit = 2,             // Alternativa 3: Fuga a pie por callejones o azoteas
        VehiclePursuit = 3,          // Alternativa 4: Fuga en vehiculo / Persecucion a motor
        SevereCrimeTwist = 4         // Alternativa 5: Giro critico mayor / Crimen encubierto (armas de fuego, rehenes, drogas)
    }

    public enum ECalloutPhase
    {
        EnRoute,
        Staging,
        Arrival,
        Active,
        Resolution,
        Cleanup
    }
}
