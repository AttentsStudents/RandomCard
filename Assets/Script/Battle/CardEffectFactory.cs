using System;
using System.Collections.Generic;
using static CardEffects;

public static class CardEffectFactory
{
    private static readonly Dictionary<string, Func<float[], ICardEffect>> effectRegistry =
        new Dictionary<string, Func<float[], ICardEffect>>
        {
            { "HealthSacrificeAttack", parameters => new HealthSacrificeAttack(parameters[0], parameters[1]) },
            { "StatusEffect", parameters => new StatusEffect("Poison", parameters[0]) }
        };

    public static ICardEffect CreateEffect(string effectClassName, float[] parameters)
    {
        if (effectRegistry.ContainsKey(effectClassName))
        {
            return effectRegistry[effectClassName](parameters);
        }

        throw new ArgumentException($"Effect class '{effectClassName}' not registered in the factory.");
    }
}

