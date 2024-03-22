#version 400 core
out vec4 FragColor;

in float dist;

uniform vec4 backgroundColor;
uniform vec4 gridColor;

void main()
{
    //FragColor = vec4(1.0f,1.0f,1.0f,1.0f);

    float fogLen = 20.0f;
    float intens = (fogLen - dist)/fogLen;
    intens = 1.0 - clamp(intens,0.0,1.0);

    //if(dist > 0 && dist < 10)
    //{
     //   FragColor = vec4(1.0f,1.0f,1.0f,1.0f);
    //} 
    //else if(dist > 10.0f)
    //{
     //   FragColor = vec4(0.5f,0.5f,0.5f,1.0f);
    //} 
    //else if(dist < 0)
    //{
     //   FragColor = vec4(0.0f,1.0f,0.0f,1.0f);
    //}
    //else
    //{
        //FragColor = mix(vec4(0.82f, 0.82f, 0.82f, 1.0f),backgroundColor,intens);
        FragColor = mix(gridColor,backgroundColor,intens);
        //FragColor = backgroundColor;
    //} 
}