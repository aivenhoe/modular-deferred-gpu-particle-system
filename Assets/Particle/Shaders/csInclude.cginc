#define thread_group_size_x 128
#define thread_group_size_y 1
struct particle
{
	float3 pos;
	float3 acc;
	float3 vel;
	float mass;
	int age;
	float4 col;
};
RWStructuredBuffer<particle> output;
