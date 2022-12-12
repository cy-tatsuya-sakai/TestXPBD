
public interface IConstraint
{
    public void InitLambda();
    public void SolvePosition(float dt);
    public void SolveVelocity(float dt, float dampCoeff);

}
