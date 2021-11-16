using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class VoteCommentParser
    {
        // {"comment_id":1253922,"comment_score":-19,"comment_vote":0}
        public static Result parse(String body, int vote)
        {
            var result = new Result();
            var jo = new JObject(body);
            result.id = jo.Value<long>("comment_id");
            result.score = jo.Value<int>("comment_score");
            result.vote = jo.Value<int>("comment_vote");
            result.expectVote = vote;
            return result;
        }

        public class Result
        {
            public long id;
            public int score;
            public int vote;
            public int expectVote;
        }
    }
}
