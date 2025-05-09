using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.Libs.Generators;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu
{
	/*!
	 * Spracovava zoznam objednavok a stara sa o postupne vykonavanie prace na objednavkach
	 */
	//meta! id="1"
	public class AgentModelu : OSPABA.Agent
	{
        public List<Objednavka> ObjednavkyTotal { get; set; } = new();
        public Dictionary<int, Objednavka> ObjednavkyWorkStarted { get; set; } = new();

        public int ObjednavkyCount => ObjednavkyTotal.Count;
        public int ObjednavkyNotWorkingOnCount => ObjednavkyTotal.Count - ObjednavkyWorkStarted.Count;
        public int ObjednavkyDoneCount { get; set; } = 0;
        public Statistics ObjednavkaTotalTime { get; } = new Statistics();

		public AgentModelu(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();

			// Setup component for the next replication
            ObjednavkyTotal.Clear();
            ObjednavkyWorkStarted.Clear();
            ObjednavkyDoneCount = 0;

            // posli inicializacnu spravu Agentovi okolia
            var message = new MyMessage(MySim);
            message.Code = Mc.NoticeInicialuzuj;
            message.Addressee = MySim.FindAgent(SimId.AgentOkolia);
            MyManager.Notice(message);

            // clear experiment statistics
            ObjednavkaTotalTime.Reset();
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerModelu(SimId.ManagerModelu, MySim, this);
			AddOwnMessage(Mc.NoticePrichodObjednavka);
			AddOwnMessage(Mc.RequestResponseVykonajOperaciu);
			AddOwnMessage(Mc.RequestResponsePriradPracovisko);
		}
		//meta! tag="end"
	}
}
