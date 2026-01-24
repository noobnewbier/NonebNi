This assembly exists solely because of Unity doesn't support having Component script in Editor assemblies,
So we need to have an additional assembly where its only job is to hold the auxiliary component script that the Editor assembly uses,

All component here should be stripped on Build/Play.  