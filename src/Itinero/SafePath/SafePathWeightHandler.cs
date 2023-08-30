using Itinero.Algorithms.Weights;
using Itinero.Graphs.Directed;
using Itinero.Profiles;
using System;

namespace Itinero.Safety
{
    /// <summary>
    /// Weight handler class that includes into consideration
    /// the safety score of the route.
    /// </summary>
    public class SafePathWeightHandler : DefaultWeightHandler
    {
        private readonly SafeScoreHandler scoreHandler;


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public SafePathWeightHandler(Func<ushort, Factor> getFactor, SafeScoreHandler scoreHandler) :
            base(getFactor)
        {
            this.scoreHandler = scoreHandler;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float Calculate(ushort edgeProfile, float distance, uint edgeId, out Factor factor)
        {
            factor = _getFactor(edgeProfile);
            var safeScore = scoreHandler.GetScore(edgeId);
            if (safeScore != null) factor.Value = safeScore.Value;
            return (distance * factor.Value);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override WeightAndDir<float> CalculateWeightAndDir(ushort edgeProfile, float distance, uint edgeId, out bool accessible)
        {
            var value = base.CalculateWeightAndDir(edgeProfile, distance, edgeId, out accessible);
            var safeScore = scoreHandler.GetScore(edgeId);
            if (safeScore != null) value.Weight = safeScore.Value;
            return value;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float GetEdgeWeight(DirectedMetaGraph.EdgeEnumerator edge, out bool? direction)
        {
            float weight;
            Data.Contracted.Edges.ContractedEdgeDataSerializer.Deserialize(edge.Data0,
                out weight, out direction);

            var safeScore = scoreHandler.GetScore(edge.Id);
            return safeScore ?? weight;
        }

        /// <summary>
        /// Gets the weight from the given edge and sets the direction.
        /// </summary>
        public override WeightAndDir<float> GetEdgeWeight(DirectedMetaGraph.EdgeEnumerator edge)
        {
            var weight = Data.Contracted.Edges.ContractedEdgeDataSerializer.Deserialize(edge.Data0);
            var safeScore = scoreHandler.GetScore(edge.Id);
            if (safeScore != null)
                weight.Weight = safeScore.Value;
            return weight;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float GetEdgeWeight(DynamicEdge edge, out bool? direction)
        {
            float weight;
            Data.Contracted.Edges.ContractedEdgeDataSerializer.Deserialize(edge.Data[0],
                out weight, out direction);

            var safeScore = scoreHandler.GetScore(edge.Id);
            return safeScore ?? weight;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float GetEdgeWeight(DirectedDynamicGraph.EdgeEnumerator edge, out bool? direction)
        {
            float weight;
            Data.Contracted.Edges.ContractedEdgeDataSerializer.Deserialize(edge.Data0,
                out weight, out direction);

            var safeScore = scoreHandler.GetScore(edge.Id);
            return safeScore ?? weight;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float GetEdgeWeight(MetaEdge edge, out bool? direction)
        {
            var weight = base.GetEdgeWeight(edge, out direction);
            var safeScore = scoreHandler.GetScore(edge.Id);
            if (safeScore != null) weight *= safeScore.Value;
            return weight;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override float Add(float weight, ushort edgeProfile, uint edgeId, float distance, out Factor factor)
        {
            factor = _getFactor(edgeProfile);
            var safeScore = scoreHandler.GetScore(edgeId);
            if (safeScore != null) factor.Value = safeScore.Value;
            return weight + (distance * factor.Value);
        }

    }
}