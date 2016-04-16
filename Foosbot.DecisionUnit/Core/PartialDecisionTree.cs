using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    public class PartialDecisionTree : DecisionTree
    {
        /// <summary>
        /// Decision Tree Constructor for tree without Subtree
        /// </summary>
        /// <param name="decisionHelper">Decision Helper Instance [default is null then will be constructed using Configuration File]</param>
        /// <param name="ballRadius">Ball Radius in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="tableWidth">Table Width (Y Axe) in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="playerWidth">Player Width in mm [default is -1 will be taken from Configuration File]</param>
        public PartialDecisionTree(IDecisionHelper helper = null, int ballRadius = -1, int tableWidth = -1, int playerWidth = -1)
            :base(helper, ballRadius, tableWidth, playerWidth)
        {

        }

        /// <summary>
        /// Main Decision Flow Method desides on action and sets property of responding player
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        public override RodAction Decide(IRod rod, BallCoordinates bfc)
        {
            //Set Default responding player
            RespondingPlayer = -1;

            return new RodAction(rod.RodType);
        }
    }
}
