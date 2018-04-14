inline float4 AlignToPixelGrid(float4 vertex)
{
    float _pixelsPerUnit = 32.0;

    float4 worldPos = mul(unity_ObjectToWorld, vertex);

    worldPos.x = floor(worldPos.x * _pixelsPerUnit + 0.5) / _pixelsPerUnit;
    worldPos.y = floor(worldPos.y * _pixelsPerUnit + 0.5) / _pixelsPerUnit;

    return mul(unity_WorldToObject, worldPos);
}