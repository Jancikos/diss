using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu
{
	/*!
	 * Spracovava zoznam objednavok a stara sa o postupne vykonavanie prace na objednavkach
	 */
	//meta! id="1"
	public class AgentModelu : OSPABA.Agent
	{
		public AgentModelu(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
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
