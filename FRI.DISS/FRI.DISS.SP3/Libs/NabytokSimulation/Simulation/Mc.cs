using FRI.DISS.Libs.Generators;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using OSPABA;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
	public class Mc : OSPABA.IdList
	{
		//meta! userInfo="Generated code: do not modify", tag="begin"
		public const int RequestResponsePriradPracovisko = 1014;
		public const int NoticePracoviskoUvolnene = 1016;
		public const int NoticePrichodObjednavka = 1001;
		public const int NoticeInicialuzuj = 1002;
		public const int NoticeStolarUvolneny = 1018;
		public const int RequestResponseVykonajOperaciu = 1006;
		public const int RequestResponseDajStolara = 1008;
		public const int RequestResponsePresunPracoviska = 1023;
		public const int RequestResponsePresunSklad = 1024;
		//meta! tag="end"

		// 1..1000 range reserved for user
        public static int GetAgentByStolarType(StolarType type)
        {
            return type switch
            {
                StolarType.A => SimId.AgentStolariA,
                StolarType.B => SimId.AgentStolariB,
                StolarType.C => SimId.AgentStolariC,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
	}
}
