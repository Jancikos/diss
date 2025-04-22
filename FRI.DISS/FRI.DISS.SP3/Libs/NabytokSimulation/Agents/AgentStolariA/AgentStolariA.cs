using OSPABA;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA
{
    //meta! id="11"
    public class AgentStolariA : OSPABA.Agent
    {
        public Dictionary<int, Stolar> Stolari = new();
        public Queue<Stolar> FreeStolarai = new();

        public AgentStolariA(int id, OSPABA.Simulation mySim, Agent parent) :
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
            for (int i = 0; i < ((MySimulation)MySim).StolariCount[StolarType.A]; i++)
            {
                var stolar = new Stolar() { Type = StolarType.A };
                Stolari.Add(stolar.Id, stolar);
                FreeStolarai.Enqueue(stolar);
            }
        }

        //meta! userInfo="Generated code: do not modify", tag="begin"
        private void Init()
        {
            new ManagerStolariA(SimId.ManagerStolariA, MySim, this);
            AddOwnMessage(Mc.RequestResponseDajStolara);
            AddOwnMessage(Mc.NoticeStolarUvolneny);
        }
        //meta! tag="end"
    }
}
