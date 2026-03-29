#ifndef NONEB_NORMALIZE_INCLUDED
#define NONEB_NORMALIZE_INCLUDED

void NormalizeToRange_float(float input, float2 currentRange, float2 newRange, out float output)
{
    /*
     * Note: https://stackoverflow.com/questions/1471370/normalizing-from-0-5-1-to-0-1#comment28467427_1477265
     * 
     *      (D-C)*(X-A)
     * X' = -----------  + C
     *        (B-A)
     */

    output = (newRange.y - newRange.x) * (input - currentRange.x) / (currentRange.y - currentRange.x) + newRange.x;
}

#endif
