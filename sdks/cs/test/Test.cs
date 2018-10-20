using ai;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;

namespace test
{
    public class Test
    {

        [Fact]
        public void Deserialize_Game_Message()
        {
            const string input = @"{""board"":[[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,1,0,0,0],[0,0,0,1,1,0,0,0],[0,0,0,2,1,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0]],""maxTurnTime"":15000,""player"":2}";
            var obj = JsonConvert.DeserializeObject<GameMessage>(input);

            obj.maxTurnTime.Should().Be(15000);
            obj.player.Should().Be(2);
            obj.board.Length.Should().Be(8);
            obj.board[0].Length.Should().Be(8);
            obj.board[0][0].Should().Be(0);
            obj.board[3][3].Should().NotBe(0);

        }

        [Fact]
        public void FindMoves()
        {
            const string input = @"{""board"":[
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,0,0,0],
                [0,0,0,1,2,0,0,0],
                [0,0,0,2,1,0,0,0],
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,0,0,0]],""maxTurnTime"":15000,""player"":2}";
            var obj = JsonConvert.DeserializeObject<GameMessage>(input);

            List<int[]> moves = AI.GetPossibleMoves(obj.board, obj.player);

            foreach (int[] move in moves)
            {
                moves.Count.Should().Be(4);
                moves.Exists(f => AI.CompareMoves(f, new int[] { 2, 3 })).Should().Be(true);
                moves.Exists(f => AI.CompareMoves(f, new int[] { 3, 2 })).Should().Be(true);
                moves.Exists(f => AI.CompareMoves(f, new int[] { 5, 4 })).Should().Be(true);
                moves.Exists(f => AI.CompareMoves(f, new int[] { 4, 5 })).Should().Be(true);
            }
        }

        [Fact]
        public void MakeMove()
        {
            const string input = @"{""board"":[
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,2,0,0],
                [0,0,1,0,1,2,0,0],
                [0,0,0,1,1,2,0,0],
                [0,0,1,2,1,2,0,0],
                [0,0,2,0,0,0,0,0],
                [0,0,0,0,0,0,0,0],
                [0,0,0,0,0,0,0,0]],""maxTurnTime"":15000,""player"":2}";
            var obj = JsonConvert.DeserializeObject<GameMessage>(input);
            AI.MakeMove(obj.board, new int[] { 2, 3 }, 2);
            obj.board[2][3].Should().Be(2);
            obj.board[2][4].Should().Be(2);

            AI.MakeMove(obj.board, new int[] { 2, 6 }, 1);
            for (int i = 2; i < 7; i++)
            {
                obj.board[2][i].Should().Be(1);
            }
        }
    }
}
