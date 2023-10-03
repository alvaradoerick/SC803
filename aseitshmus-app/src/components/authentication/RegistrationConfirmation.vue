<script setup>
    import {
        useRouter
    } from 'vue-router';
    const router = useRouter();
    const ladingPage = () => {
        router.push({
            name: "login"
        });
    }
</script>
<template>
    <div>
        <h2>Solicitud enviada</h2>
<p>Su solicitud ha sido enviada para revisión. Le informamos que se requiere que establezca una nueva contraseña una vez que su cuenta haya sido activada.</p>
<p>Si tiene menos de 3 meses trabajando en la empresa, su solicitud quedará pendiente hasta que cumpla los <strong>3 meses.</strong></p>
<div>
    <p>Una vez que el administrador apruebe su solicitud, podrá acceder a su cuenta y proceder a cambiar su contraseña.</p>
</div>
        <div class="form-row">
            <a class="links" @click="ladingPage">
                Página Principal
            </a>
        </div>
    </div>
</template>

<style scoped>
    h2 {
        color: #077CBC;
        text-align: center;
        margin-top: 25px;
        margin-bottom: 50px;
    }

    .form-row {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-bottom: 3rem;
        width: 100%;
    }

    .links {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-top: 3rem;
        width: 100%;
    }
</style>