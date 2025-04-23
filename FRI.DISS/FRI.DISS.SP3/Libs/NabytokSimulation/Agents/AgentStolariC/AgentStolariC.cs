using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariC
{
	//meta! id="13"
    public class AgentStolariC : OSPABA.Agent, IAgentStolari
    {
        public Dictionary<int, Stolar> Stolari { get; } = new();
        public Queue<Stolar> FreeStolarai { get; } = new();

		public AgentStolariC(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();

            // Setup component for the next replication
            FreeStolarai.Clear();
            Stolari.Clear();
            for (int i = 0; i < ((MySimulation)MySim).StolariCount[StolarType.C]; i++)
            {
                var stolar = new Stolar() { Type = StolarType.C };
                Stolari.Add(stolar.Id, stolar);
                FreeStolarai.Enqueue(stolar);
            }
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerStolariC(SimId.ManagerStolariC, MySim, this);
			AddOwnMessage(Mc.RequestResponseDajStolara);
			AddOwnMessage(Mc.NoticeStolarUvolneny);
		}
		//meta! tag="end"
	}
}
