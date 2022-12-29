#version 450 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float shininess;
};

struct Light {
    vec3 position;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 FragColor;
  
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform Material material;
uniform Light light;

in vec3 fragPos;

void main()
{
    vec3 normal = normalize(cross(dFdx(fragPos),dFdy(fragPos)));
    normal = normalize(normal);
    vec3 lightDir = normalize(lightPos); 
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 bisector = normalize(viewDir + lightDir);
    float spec = pow(max(dot(bisector, normal), 0.0), material.shininess);

    vec3 ambient = light.ambient * material.ambient;
    vec3 diffuse = light.diffuse * (diff * material.diffuse);
    vec3 specular = light.specular * (spec * material.specular);

    vec3 result = ambient + diffuse + spec;
    FragColor = vec4(result, 1.0);
}

