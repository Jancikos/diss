

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
        CakaNaZaciatokPrace,
        Narezana,
        Namorena,
        Nalakovana,
        Poskladana,
        NamontovaneKovania,
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

        /// <summary>
        /// ci su vsetky prace na nabytku hotove
        /// </summary>
        /// <value></value>
        public bool AllWorkDone
        {
            get
            {
                switch (State)
                {
                    case NabytokState.Poskladana:
                        return Type == NabytokType.Skrina
                            ? false
                            : true;
                    case NabytokState.NamontovaneKovania:
                    case NabytokState.Ukoncena:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public Pracovisko? Pracovisko { get; set; } = null;

        public Nabytok(Objednavka objednavka, NabytokType type)
        {
            Objednavka = objednavka;
            Type = type;
        }

        public override string ToString()
        {
            return $"#{Id} {Type} [{State}]";
        }

        public NabytokState GetNextState()
        {
            switch (State)
            {
                case NabytokState.CakaNaZaciatokPrace:
                    return NabytokState.Narezana;
                case NabytokState.Narezana:
                    return NabytokState.Namorena;
                case NabytokState.Namorena:
                    return NabytokState.Nalakovana;
                case NabytokState.Nalakovana:
                    return NabytokState.Poskladana;
                case NabytokState.Poskladana:
                    if (Type == NabytokType.Skrina)
                        return NabytokState.NamontovaneKovania;
                    break;
            }
            return NabytokState.Ukoncena;
        }

        public NabytokOperation MapStateToNextOperation()
        {
            switch (State)
            {
                case NabytokState.CakaNaZaciatokPrace:
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

        public StolarType[] MapOperationToStolarTypes() => MapOperationToStolarTypes(MapStateToNextOperation());
        public static StolarType[] MapOperationToStolarTypes(NabytokOperation operation)
        {
            switch (operation)
            {
                case NabytokOperation.Rezanie:
                    return [StolarType.A];
                case NabytokOperation.Morenie:
                case NabytokOperation.Lakovanie:
                    return [StolarType.C];
                case NabytokOperation.Skladanie:
                    return [StolarType.B];
                case NabytokOperation.MontazKovani:
                    return [StolarType.A, StolarType.C];
            }
            throw new NotImplementedException($"Operation {operation} is not implemented");
        }
        public static NabytokOperation[] MapStolarTypeToOperations(StolarType stolarType)
        {
            switch (stolarType)
            {
                case StolarType.A:
                    return [NabytokOperation.Rezanie, NabytokOperation.MontazKovani];
                case StolarType.B:
                    return [NabytokOperation.Skladanie];
                case StolarType.C:
                    return [NabytokOperation.Morenie, NabytokOperation.Lakovanie, NabytokOperation.MontazKovani];
            }
            throw new NotImplementedException($"Stolar type {stolarType} is not implemented");
        }
    }
}