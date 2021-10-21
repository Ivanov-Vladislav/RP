namespace Common
{
    public struct SimilarityMessage
    {
        public string Id { get; set; }
        public double Similarity { get; set; }

        public SimilarityMessage(string id, double similarity) 
        {
            Id = id;
            Similarity = similarity;
        }
    }
}