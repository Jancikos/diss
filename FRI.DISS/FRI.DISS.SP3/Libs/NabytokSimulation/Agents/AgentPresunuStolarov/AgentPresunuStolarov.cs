using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov.ContinualAssistants;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov
{
	/*!
	 * bude primat 2 ReqRRes
	 * 
	 * presun medzi skldom a wp
	 * presun mediz wp a wp 
	 */
	//meta! id="54"
	public class AgentPresunuStolarov : OSPABA.Agent
	{
		public AgentPresunuStolarov(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerPresunuStolarov(SimId.ManagerPresunuStolarov, MySim, this);
			new ProcessPresunSklad(SimId.ProcessPresunSklad, MySim, this);
			new ProcessPresunPracoviska(SimId.ProcessPresunPracoviska, MySim, this);
			AddOwnMessage(Mc.RequestResponsePresunSklad);
			AddOwnMessage(Mc.RequestResponsePresunPracoviska);
		}
		//meta! tag="end"
	}
}
