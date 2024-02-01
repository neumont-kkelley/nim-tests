namespace NimTests
{

    public interface IPuzzle
    {
        string Question { get; }
        List<string> Choices { get; }
        string Answer { get; }

    }
    public interface IArea
    {
        string Name { get; }

        List<IArea> AvailableAreas { get; }

        IPuzzle Puzzle { get; }
    }

    public class MathPuzzle : IPuzzle
    {
        public string Question => "What is 1 + 1?";

        private string _answer = "2";
        public List<string> Choices => new List<string>() { "1", "3", _answer };

        public string Answer => _answer;

    }

    public class RiddlePuzzle : IPuzzle
    {
        public string Question => "What is blue?";

        private string _answer = "Sky";
        public List<string> Choices => new List<string>() { "Earth", _answer, "Summer" };

        public string Answer => _answer;
    }

    public class NoPuzzle : IPuzzle
    {
        public string Question => "";

        public List<string> Choices => new List<string>();

        public string Answer => "";
    }

    public class Cave : IArea
    {
        public Cave() { }
        public string Name => "Cave";
        public List<IArea> AvailableAreas => new() { };

        public IPuzzle Puzzle => new NoPuzzle();
    }

    public class Forrest : IArea
    {
        public Forrest() { }
        public string Name => "Forrest";
        public List<IArea> AvailableAreas => new() { new Cave(), new City() };

        public IPuzzle Puzzle => new NoPuzzle();
    }

    public class City : IArea
    {
        public City() { }

        public string Name => "City";

        public List<IArea> AvailableAreas => new() { new Tower(), new Library(), new Forrest() };
        public IPuzzle Puzzle => new NoPuzzle();
    }

    public class Tower : IArea
    {
        public Tower() { }

        public string Name => "Tower";

        public List<IArea> AvailableAreas => new() { new City() };
        public IPuzzle Puzzle => new RiddlePuzzle();
    }

    public class Library : IArea
    {
        public Library() { }

        public string Name => "Library";
        public List<IArea> AvailableAreas => new() { new City(), new EndArea() };

        public IPuzzle Puzzle => new MathPuzzle();
    }

    public class EndArea : IArea
    {
        public EndArea() { }

        public string Name => "EndArea";
        public List<IArea> AvailableAreas => new() { };
        public IPuzzle Puzzle => new NoPuzzle();
    }

    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public class Game
    {
        public GameState GameState { get; private set; }

        public IArea CurrentArea { get; private set; }
        public Game()
        {
            GameState = GameState.Playing;
            CurrentArea = new Forrest();
        }

        public void MoveToArea(IArea area)
        {
            if (GameState == GameState.Playing)
            {
                foreach (var available in CurrentArea.AvailableAreas)
                {
                    if (available.Name == area.Name)
                    {
                        CurrentArea = area;

                        if (CurrentArea is EndArea)
                        {
                            GameState = GameState.Win;
                        }

                        if (CurrentArea is Cave)
                        {
                            GameState = GameState.Lose;
                        }

                        return;
                    }
                }

                throw new Exception("INVALID AREA");
            }

            throw new Exception("GAME OVER");
        }
    }

    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------
    //--------------------------------------------------------------


    public class GameUnitTests
    {

        [Fact]
        public void Game_Should_Solve_RiddlePuzzle()
        {
            var puzzle = new RiddlePuzzle();

            Assert.True(puzzle.Question == "What is blue?");
            Assert.True(puzzle.Answer == "Sky");

            var correctAnswers = 1;
            var foundAnswers = 0;
            foreach (var x in puzzle.Choices)
            {
                if (x == puzzle.Answer)
                {
                    foundAnswers++;
                }
            }

            Assert.True(correctAnswers == foundAnswers);
            Assert.True(puzzle.Choices.Count == 3);
            Assert.Contains(puzzle.Answer, puzzle.Choices);
            Assert.Contains("Earth", puzzle.Choices);
            Assert.Contains("Summer", puzzle.Choices);
        }


        [Fact]
        public void Game_Should_Solve_MathPuzzle()
        {
            var puzzle = new MathPuzzle();

            Assert.True(puzzle.Question == "What is 1 + 1?");
            Assert.True(puzzle.Answer == "2");

            var correctAnswers = 1;
            var foundAnswers = 0;
            foreach (var x in puzzle.Choices)
            {
                if (x == puzzle.Answer)
                {
                    foundAnswers++;
                }
            }

            Assert.True(correctAnswers == foundAnswers);
            Assert.True(puzzle.Choices.Count == 3);
            Assert.Contains(puzzle.Answer, puzzle.Choices);
            Assert.Contains("1", puzzle.Choices);
            Assert.Contains("3", puzzle.Choices);
        }

        [Fact]
        public void Game_Should_Solve_NoPuzzle()
        {
            var puzzle = new NoPuzzle();

            Assert.True(puzzle.Choices.Count == 0);
            Assert.True(puzzle.Answer == "");
            Assert.True(puzzle.Question == "");
        }

        [Fact]
        public void Game_Should_Validate_Areas()
        {
            IArea area = new Forrest();
            Assert.True(area.Name == "Forrest");
            Assert.True(area.AvailableAreas.Count == 2);
            Assert.True(area.AvailableAreas[0] is Cave);
            Assert.True(area.AvailableAreas[1] is City);
            Assert.True(area.Puzzle is NoPuzzle);

            area = new Cave();
            Assert.True(area.Name == "Cave");
            Assert.True(area.AvailableAreas.Count == 0);
            Assert.True(area.Puzzle is NoPuzzle);

            area = new City();
            Assert.True(area.Name == "City");
            Assert.True(area.AvailableAreas.Count == 3);
            Assert.True(area.AvailableAreas[0] is Tower);
            Assert.True(area.AvailableAreas[1] is Library);
            Assert.True(area.AvailableAreas[2] is Forrest);
            Assert.True(area.Puzzle is NoPuzzle);

            area = new Tower();
            Assert.True(area.Name == "Tower");
            Assert.True(area.AvailableAreas.Count == 1);
            Assert.True(area.AvailableAreas[0] is City);
            Assert.True(area.Puzzle is RiddlePuzzle);

            area = new Library();
            Assert.True(area.Name == "Library");
            Assert.True(area.AvailableAreas.Count == 2);
            Assert.True(area.AvailableAreas[0] is City);
            Assert.True(area.AvailableAreas[1] is EndArea);
            Assert.True(area.Puzzle is MathPuzzle);

            area = new EndArea();
            Assert.True(area.Name == "EndArea");
            Assert.True(area.AvailableAreas.Count == 0);
            Assert.True(area.Puzzle is NoPuzzle);
        }

        [Fact]
        public void Game_Should_Path_Correctly()
        {
            /*
             * Areas: Cave, Forrest, City, Tower, Library, EndArea
             * Path: 
             * Forrest -> Cave=LOSE, City
             * City -> Forrest, Tower, Library
             * Tower -> City
             * Library -> City, EndArea=WIN
             *
             */

            var game = new Game();
            Assert.True(game.CurrentArea is Forrest);
            Assert.True(game.GameState == GameState.Playing);

            Assert.Throws<Exception>(() => game.MoveToArea(new Tower()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Library()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));

            game.MoveToArea(new Cave());
            Assert.True(game.CurrentArea.Puzzle is NoPuzzle);

            //Can't move anymore, game over
            Assert.Throws<Exception>(() => game.MoveToArea(new Forrest()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Library()));
            Assert.Throws<Exception>(() => game.MoveToArea(new City()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Tower()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));

            Assert.True(game.CurrentArea is Cave);
            Assert.True(game.GameState == GameState.Lose);


            //Start a new game
            game = new Game();
            Assert.True(game.CurrentArea is Forrest);
            Assert.True(game.GameState == GameState.Playing);
            Assert.Throws<Exception>(() => game.MoveToArea(new Tower()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Library()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 2);

            game.MoveToArea(new City());
            Assert.True(game.CurrentArea is City);
            Assert.True(game.CurrentArea.Puzzle is NoPuzzle);
            Assert.Throws<Exception>(() => game.MoveToArea(new Cave()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 3);

            game.MoveToArea(new Tower());
            Assert.True(game.CurrentArea is Tower);
            Assert.True(game.CurrentArea.Puzzle is RiddlePuzzle);
            Assert.Throws<Exception>(() => game.MoveToArea(new Library()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Cave()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Forrest()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 1);

            game.MoveToArea(new City());
            Assert.True(game.CurrentArea is City);
            Assert.True(game.CurrentArea.Puzzle is NoPuzzle);
            Assert.Throws<Exception>(() => game.MoveToArea(new Cave()));
            Assert.Throws<Exception>(() => game.MoveToArea(new EndArea()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 3);

            game.MoveToArea(new Library());
            Assert.True(game.CurrentArea is Library);
            Assert.True(game.CurrentArea.Puzzle is MathPuzzle);
            Assert.Throws<Exception>(() => game.MoveToArea(new Cave()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Tower()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Forrest()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 2);

            game.MoveToArea(new EndArea());
            Assert.True(game.CurrentArea is EndArea);
            Assert.True(game.CurrentArea.Puzzle is NoPuzzle);
            Assert.Throws<Exception>(() => game.MoveToArea(new Cave()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Tower()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Forrest()));
            Assert.Throws<Exception>(() => game.MoveToArea(new Library()));
            Assert.Throws<Exception>(() => game.MoveToArea(new City()));
            Assert.True(game.CurrentArea.AvailableAreas.Count() == 0);

            //Win the game
            Assert.True(game.GameState == GameState.Win);

        }
    }
}