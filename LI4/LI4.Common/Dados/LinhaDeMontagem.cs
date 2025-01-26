namespace LI4.Common.Dados;

public class LinhaDeMontagem {
    public int nStages;
    public Estagio[] stages;
    public List<int> waitingConstructions;   // idConstruction

    public LinhaDeMontagem(int nStages, Estagio[] stages) {
        this.nStages = nStages;
        this.stages = stages;
        this.waitingConstructions = new();
    }

}

public class Estagio {
    public int? idConstruction;
    public TimerWrapper? tw;
    public int tempo;
    public bool tried;

    public Estagio() {
        this.idConstruction = null;
        this.tried = false;
    }

    public void setupTimer(int seconds) {
        this.tempo = seconds;
    }

    public Estagio DeepClone() {
        return new Estagio {
            idConstruction = this.idConstruction,
            tw = null,
            tempo = this.tempo,
            tried = this.tried
        };
    }
}