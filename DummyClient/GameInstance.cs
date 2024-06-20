using ServerCore.Network;

namespace DummyClient
{
    class GameInstance
    {
        public static GameInstance Instance { get; } = new GameInstance();

        public GameSession Session { get; set; } = null;

        private GameInstance()
        { }

        public void Enter()
        {
            Console.Write("ID : ");
            Session.Name = Console.ReadLine();

            Session.Send(ClientPacketHandler.Instance.MakeC_Enter(Session.Name));
        }

        public void HandleEnter(bool success)
        {
            if (success)
            {
                Session.Entered = true;
                Console.WriteLine("===== Enter Room =====");
            }
            else
            {
                Console.WriteLine("ID Duplicated.");
                Enter();
            }
        }

        public void Exit()
        { 
            Session.Send(ClientPacketHandler.Instance.MakeC_Exit(Session.Name));
        }

        public void HandleExit(bool success)
        {
            if (success)
            {
                Session.Entered = false;
                Console.WriteLine("===== Exit Room ====="); ;
            }
            else
            {
                Exit();
            }
        }
    }
}
