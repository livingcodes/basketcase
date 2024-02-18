namespace Basketcase;
public interface ICache
{
  T Get<T>(str key);
  void Set(str key, obj val, int sec);
}