using UnityEngine;

public class LifeformBreedState: IState<Lifeform>
{
    private static LifeformBreedState m_Instance = new LifeformBreedState();
    public static LifeformBreedState Instance => m_Instance;

    public string Identifier => "Breeding";

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf)
    {
        lf.Navigation.ResetPath();
        lf.Navigation.Stop();
    }

    private Lifeform GetBreedingInterest(Lifeform lf)
    {
        foreach(LifeformInterest i in lf.Interests.GetInterests())
        {
            if(i.Intent == LifeformIntent.Breed)
                return i.Lifeform;
        }

        return null;
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.Delta();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        Lifeform other = GetBreedingInterest(lf);
        if(other != null) 
        {
            other.Navigation.Stop();
            other.Navigation.ResetPath();
            other.DeltaEnergy(-other.Energy + 0.1f);

            lf.Breed(other);
            lf.Interests.RemoveAllWith(LifeformIntent.Breed);
            lf.DeltaEnergy(-lf.Energy + 0.1f);
        }
         
        return LifeformIdleState.Instance;
    }
}
