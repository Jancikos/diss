

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public enum NabytokType
    {
        Stol,
        Stolicka,
        Skrina
    }

    public enum NabytokOperation
    {
        Rezanie,
        Morenie,
        Lakovanie,
        Skladanie,
        MontazKovani
    }

    public enum NabytokState
    {
        CakaNaPracovisko,
        CakaNaNarezanie,
        Narezana,
        Namorena,
        Nalakovana,
        Poskladana,
        Ukoncena
    }

    public class Nabytok
    {
        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => IdCounter++;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public Objednavka Objednavka { get; init; }

        public NabytokType Type { get; init; }
        public NabytokState State { get; set; } = NabytokState.CakaNaPracovisko;

        public Pracovisko? Pracovisko { get; set; } = null;

        public Nabytok(Objednavka objednavka, NabytokType type)
        {
            Objednavka = objednavka;
            Type = type;
        }

        public NabytokOperation MapStatusToNextOperation()
        {
            switch (State)
            {
                case NabytokState.CakaNaNarezanie:
                    return NabytokOperation.Rezanie;
                case NabytokState.Narezana:
                    return NabytokOperation.Morenie;
                case NabytokState.Namorena:
                    return NabytokOperation.Lakovanie;
                case NabytokState.Nalakovana:
                    return NabytokOperation.Skladanie;
                case NabytokState.Poskladana:
                    if (Type == NabytokType.Skrina)
                        return NabytokOperation.MontazKovani;
                    break;
            }
            throw new NotImplementedException();
        }

        public static StolarType MapOperationToStolarType(NabytokOperation operation)
        {
            return operation switch
            {
                NabytokOperation.Rezanie => StolarType.A,
                NabytokOperation.Morenie => StolarType.C,
                NabytokOperation.Skladanie => StolarType.B,
                NabytokOperation.MontazKovani => StolarType.C,
                _ => throw new NotImplementedException()
            };

        }
    }
}