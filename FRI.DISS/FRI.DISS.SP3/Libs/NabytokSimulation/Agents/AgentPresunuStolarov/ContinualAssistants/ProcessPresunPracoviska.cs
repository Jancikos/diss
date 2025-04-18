using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov.ContinualAssistants
{
	//meta! id="64"
	public class ProcessPresunPracoviska : OSPABA.Process
	{
		public ProcessPresunPracoviska(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentPresunuStolarov", id="65", type="Start"
		public void ProcessStart(MessageForm message)
		{
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Start:
				ProcessStart(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentPresunuStolarov MyAgent
		{
			get
			{
				return (AgentPresunuStolarov)base.MyAgent;
			}
		}
	}
}
