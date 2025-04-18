using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk
{
	/*!
	 * pokusi sa priradit pracovisko pre dany nabytok
	 * 
	 * ak nema aktualne volne pracovisko, tak ziadost zaradi do radu
	 * 
	 * 
	 */
	//meta! id="30"
	public class AgentPracovisk : OSPABA.Agent
	{
		public AgentPracovisk(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerPracovisk(SimId.ManagerPracovisk, MySim, this);
			AddOwnMessage(Mc.RequestResponsePriradPracovisko);
			AddOwnMessage(Mc.NoticePracoviskoUvolnene);
		}
		//meta! tag="end"
	}
}
