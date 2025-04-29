using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
using OSPAnimator;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariB
{
	//meta! id="12"
    public class AgentStolariB : OSPABA.Agent, IAgentStolari
    {
        public Dictionary<int, Stolar> Stolari { get; } = new();
        public Queue<Stolar> FreeStolarai { get; } = new();
		public AgentStolariB(int id, OSPABA.Simulation mySim, Agent parent) :
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
            for (int i = 0; i < ((MySimulation)MySim).StolariCount[StolarType.B]; i++)
            {
                var stolar = new Stolar() { Type = StolarType.B };
                Stolari.Add(stolar.Id, stolar);
                FreeStolarai.Enqueue(stolar);
            }
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerStolariB(SimId.ManagerStolariB, MySim, this);
			AddOwnMessage(Mc.RequestResponseDajStolara);
			AddOwnMessage(Mc.NoticeStolarUvolneny);
		}
		//meta! tag="end"
        public void InitializeSimulationAnimator(IAnimator oldAnimator, IAnimator newAnimator)
        {
            foreach (var stolar in Stolari.Values)
            {
                stolar.Initialize(newAnimator);
            }
        }

        public void DestroySimulationAnimator(IAnimator oldAnimator)
        {
            foreach (var stolar in Stolari.Values)
            {
                stolar.Destroy(oldAnimator);
            }
        }
	}
}
