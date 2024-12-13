using System.Collections;
using UnityEngine;

namespace mierdergames.FeedbackEffects
{

    public class FeedbackEffects : MonoBehaviour
    {
        //eventos
        public delegate void NotifyDelayStarted();
        public delegate void NotifyDelayEnded();
        public delegate void NotifyFadeInStarted();
        public delegate void NotifyFadeInEnded();
        public delegate void NotifyFadeOutStarted();
        public delegate void NotifyFadeOutEnded();
        public delegate void NotifyKickBackStarted();
        public delegate void NotifyKickBackEnded();

        public event NotifyDelayStarted DelayStarted;
        public event NotifyDelayEnded DelayEnded;
        public event NotifyFadeInStarted FadeInStarted;
        public event NotifyFadeInEnded FadeInEnded;
        public event NotifyFadeOutStarted FadeOutStarted;
        public event NotifyFadeOutEnded FadeOutEnded;
        public event NotifyKickBackStarted KickBackStarted;
        public event NotifyKickBackEnded KickBackEnded;
        //

        private Transform target;

        /// <summary>
        /// Fades time until stops, then waits for some secs, then resumes time.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        IEnumerator HitStop(float duration, int pause)
        {
            float elapsed = 0f;
            float localtimescale = Time.timeScale;
            print("Time Fading at: " + Time.time);

            while (elapsed < duration)
            {
                Time.timeScale = localtimescale - (localtimescale * elapsed / duration);

                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSecondsRealtime(pause);
            Time.timeScale = localtimescale;
            print("Time Resumes at: " + Time.time);
        }


        /// <summary>
        /// Fades time until stop, then pauses for secs, then fades in time until resumes fully.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        IEnumerator HitStopFull(float duration, int pause)
        {
            float elapsed = 0f;
            float localtimescale = Time.timeScale;
            print("Time Fading at: " + Time.time);

            while (elapsed < duration)
            {
                Time.timeScale = localtimescale - (localtimescale * elapsed / duration);

                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSecondsRealtime(pause);
            elapsed = 0f;
            while (elapsed < duration)
            {
                Time.timeScale = (localtimescale * elapsed / duration);

                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            Time.timeScale = localtimescale;
            print("Time Resumes at: " + Time.time);
        }


        /// <summary>
        /// CoRoutines that do shakes on the specified transform
        /// </summary>
        /// <param name="transf"></param>
        /// <param name="duration"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public IEnumerator Shake1(Transform transf, float duration, float magnitude)
        {
            target = transf;
            Vector3 orignalPosition = target.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                target.transform.localPosition += new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.transform.localPosition = orignalPosition;
        }

        public IEnumerator Shake2(Transform transf, float duration, float magnitude)
        {
            target = transf;
            Vector3 orignalPosition = target.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                target.transform.position = new Vector3(x, y, orignalPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.transform.position = orignalPosition;
        }

        public IEnumerator Shake3(Transform transf, Vector2 shakePreset)
        {
            target = transf;
            Vector3 orignalPosition = target.transform.position;
            float elapsed = 0f;

            while (elapsed < shakePreset.x)
            {
                float x = Random.Range(-1f, 1f) * shakePreset.y;
                float y = Random.Range(-1f, 1f) * shakePreset.y;

                target.transform.position = new Vector3(x, y, orignalPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.transform.position = orignalPosition;
        }


        /// <summary>
        /// performs a Kickback effect on the specified Transform an amount on a direction, for certain time.
        /// </summary>
        /// <param name="transf"></param>
        /// <param name="dirAmount"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator Kickback(Transform transf, Vector2 dirAmount, float duration)
        {
            float elapsed = 0f;
            Vector2 orignalPos = transf.localPosition;

            transf.position += new Vector3(dirAmount.x, dirAmount.y, transf.position.z);

            print("Kicback Started of: " + transf.name + " at: " + Time.time);
            KickBackStarted?.Invoke();

            while (elapsed < duration)
            {
                var LerpPos = Vector2.Lerp(transf.localPosition, orignalPos, elapsed);
                transf.localPosition = LerpPos;

                elapsed += Time.deltaTime;
                yield return null;
            }

            transf.position = orignalPos;
            print("Kickback Ended of: " + transf.name + " at: " + Time.time);
            KickBackEnded?.Invoke();
        }



        /// <summary>
        /// delays for some secs
        /// </summary>
        /// <param name="secs"></param>
        /// <returns></returns>
        IEnumerator Delayo(int secs)
        {
            //Print the time of when the function is first called.
            print("Started Delay at: " + Time.time);
            DelayStarted?.Invoke();


            yield return new WaitForSeconds(secs);
            //After we have waited 5 seconds print the time again.
            print("Finished Delay at: " + Time.time);
            DelayEnded?.Invoke();
        }


        /// <summary>
        /// Stops "time" using TimeScale, for a duration secs, then resumes.
        /// IMPORTANT: Only those transforms moving by Time.DeltaTime will be affected!
        /// </summary>
        /// <param name="secs"></param>
        /// <returns></returns>
        IEnumerator StopTime(float duration)
        {
            float elapsed = 0f;
            float localtimescale = Time.timeScale;
            print("Time Stops at: " + Time.time);

            Time.timeScale = 0f;
            while (elapsed < duration)
            {

                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            Time.timeScale = localtimescale;
            print("Time Resumes at: " + Time.time);
        }


        /// <summary>
        /// Fades "time" to full stop , for a duration, then resumes.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator FadeTime(float duration)
        {
            float elapsed = 0f;
            float localtimescale = Time.timeScale;
            print("Time Fading at: " + Time.time);

            while (elapsed < duration)
            {
                Time.timeScale = localtimescale - (localtimescale * elapsed / duration);

                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            Time.timeScale = localtimescale;
            print("Time Resumes at: " + Time.time);
        }



        /// <summary>
        /// performs a fadeOUT (from color to transparent) on the specified SpriteRenderer.
        /// </summary>
        /// <param name="sprComp"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator FadeOut(SpriteRenderer sprComp, float duration)
        {
            float elapsed = 0f;
            float toValue = 0f;
            print("Started FadeOut of" + sprComp.transform.name + " at: " + Time.time);
            FadeOutStarted?.Invoke();
            while (elapsed < duration)
            {
                float lerpo = Mathf.Lerp(1f - (sprComp.color.a * elapsed / duration), toValue, elapsed);
                sprComp.color = new Color(lerpo, lerpo, lerpo, lerpo);

                elapsed += Time.deltaTime;
                yield return null;
            }

            sprComp.color = Color.clear; //0,0,0,0
            print("Ended FadeOut of" + sprComp.transform.name + " at: " + Time.time);
            FadeOutEnded?.Invoke();
        }


        /// <summary>
        /// performs a fadeIN (from transparent to that original color) on the specified SpriteRenderer
        /// </summary>
        /// <param name="sprComp"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator FadeIn(SpriteRenderer sprComp, float duration)
        {
            float elapsed = 0f;
            float toValue = 1f;
            print("Started FadeIn of " + sprComp.transform.name + " at: " + Time.time);
            FadeInStarted?.Invoke();
            while (elapsed < duration)
            {
                float lerpo = Mathf.Lerp((sprComp.color.a * elapsed / duration), toValue, elapsed);
                sprComp.color = new Color(sprComp.color.r, sprComp.color.g, sprComp.color.b, lerpo);

                elapsed += Time.deltaTime;
                yield return null;
            }

            sprComp.color = new Color(sprComp.color.r, sprComp.color.g, sprComp.color.b, 1f);
            print("Ended FadeIn of " + sprComp.transform.name + " at: " + Time.time);
            FadeInEnded?.Invoke();
        }
    }
}
