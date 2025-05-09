using OSPABA;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov.ContinualAssistants;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using OSPAnimator;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov
{
    /*!
	 * ku operacii sa pokusa priradit stolara. 
	 * ak aktualne taky stolar nie je volny, tak zaradi danu operaciu do radu
	 * 
	 * tiez riadi presun stolara medzi pracoviskami alebo skladom
	 */
    //meta! id="10"
    public class AgentStolarov : OSPABA.Agent
    {
        public Dictionary<NabytokOperation, PriorityQueue<MyMessage, Nabytok>> OperationsQueues = new()
            {
                { NabytokOperation.PripravaMaterialu, new() },
                { NabytokOperation.Rezanie, new() },
                { NabytokOperation.Morenie, new() },
                { NabytokOperation.Lakovanie, new() },
                { NabytokOperation.Skladanie, new() },
                { NabytokOperation.MontazKovani, new() },
                { NabytokOperation.KontrolaUpevnenia, new() }
            };

        public AgentStolarov(int id, OSPABA.Simulation mySim, Agent parent) :
            base(id, mySim, parent)
        {
            Init();
        }

        override public void PrepareReplication()
        {
            base.PrepareReplication();
            // Setup component for the next replication

            // reset operations queues
            foreach (var operation in OperationsQueues.Keys)
            {
                OperationsQueues[operation].Clear();
            }
        }

        //meta! userInfo="Generated code: do not modify", tag="begin"
        private void Init()
        {
            new ManagerStolarov(SimId.ManagerStolarov, MySim, this);
            new ProcessVykonajOperaciu(SimId.ProcessVykonajOperaciu, MySim, this);
            AddOwnMessage(Mc.RequestResponsePresunSklad);
            AddOwnMessage(Mc.RequestResponsePresunPracoviska);
            AddOwnMessage(Mc.RequestResponseDajStolara);
            AddOwnMessage(Mc.RequestResponseVykonajOperaciu);
        }
        //meta! tag="end"
    }
}
