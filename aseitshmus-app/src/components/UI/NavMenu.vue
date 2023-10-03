<script setup>
import { computed } from "vue";
import Menubar from 'primevue/menubar';
import { useStore } from 'vuex';
import { roles } from "../../constants/RolesConst.js";
import { useRouter } from 'vue-router';

    const store = useStore();
    const router = useRouter()

    const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
    const role = computed(() => store.getters['auth/getRole']);


    const isAdmin = () => role.value == roles.ADMINISTRATOR;
    const isUser = () => role.value != roles.ADMINISTRATOR;
    const isReviewerRoles = () => role.value == roles.ADMINISTRATOR || role.value == roles.PRESIDENT || role.value == roles.TREASURER;


    const logout = async () => {
        await  store.dispatch("auth/logout");
        router.push({ name: "login" });
        };

    const items = computed(() => {
        return [
            {
                label: 'Iniciar sesión',
                icon: 'pi pi-fw pi-sign-in',
                visible: !isAuthenticated.value,
                to: { name: 'login' }
            },
            {
                label: 'Registro',
                icon: 'pi pi-fw pi-book',
                visible: !isAuthenticated.value,
                to: { name: 'register' }
            },
            {
                label: 'Inicio',
                icon: 'pi pi-fw pi-home',
                visible: isAuthenticated.value && isAdmin,
                to: { name: 'dashboard' }
            },
            {
                label: 'Inicio',
                icon: 'pi pi-fw pi-home',
                visible: isAuthenticated.value && isUser,
                to: { name: 'myDashboard' }
            },
            {
                label: 'Mi cuenta',
                icon: 'pi pi-fw pi-user',
                visible: isAuthenticated.value,
                items: [{
                        label: 'Mi perfíl',
                        to: { name: 'myProfile' }
                    },
                    {
                        label: 'Cambiar contraseña',
                        to: { name: 'changePassword' }
                    }
                ]
            },
            {
                label: 'Usuarios',
                icon: 'pi pi-fw pi-user',
                visible: isAuthenticated.value && isAdmin,
                to: { name: 'listUsers' }
            },       
            {
                label: 'Solicitudes',
                icon: 'pi pi-fw pi-dollar',
                visible: isAuthenticated.value,
                items: [
                {
                    label: 'Lista de solicitudes de préstamos',
                    visible: isReviewerRoles,
                    to: { name: 'loanRequestList' }
                },
                {
                    label: 'Lista de solicitudes de ahorros',
                    visible: isAdmin,
                    to: { name: 'savingsRequestList' }
                },
                {
                        label: 'Solicitar préstamos',
                        visible: isUser,
                        to: { name: 'requestLoan' }
                },
                {
                    label: 'Solicitar ahorros',
                    visible: isUser,
                    to: { name: 'requestSavings' }
                }
                ]
            },
            {
                label: 'Administrativo',
                icon: 'pi pi-list',
                visible: isAuthenticated.value && isAdmin,
                items: [{
                        label: 'Administración de convenios',
                        items: [{
                                label: 'Lista de categorias',
                                to: { name: 'categoryList' }
                            },
                            {
                                label: 'Lista de convenios',
                                to: { name: 'agrementList' }
                            }
                        ]
                    },
                    {
                        label: 'Préstamos',
                        visible: isAuthenticated.value && isReviewerRoles,
                        items: [{
                            label: 'Administración de tipos de préstamos',
                            visible: isAdmin,
                            to: {
                                name: 'typeList'
                            }
                        }]
                    },
                    {
                        label: 'Ahorros',
                        visible: isAuthenticated.value && isAdmin,
                        items: [{
                            label: 'Administración de tipos de ahorros',
                            to: {
                                name: 'savingsList'
                            }
                        }]
                    },
                    {
                        label: 'Carga de archivos',
                        visible: isAuthenticated.value && isAdmin,
                        to: {
                            name: 'uploadImport'
                        }
                    },
                ],
            },
            {
                label: 'Convenios',
                icon: 'pi pi-fw pi-shopping-bag',
                to: { name: 'allAgreements' }
            },
            {
                label: 'Salir',
                visible: isAuthenticated.value,
                icon: 'pi pi-fw pi-power-off',
                to: { name: 'login' },
            }
        ]
    });

    const handleMenuItemClick = async (event) => {
        const clickedMenuItem = event.target.closest('.p-menuitem');
        if (clickedMenuItem) {
            const clickedMenuItemLabel = clickedMenuItem.innerText.trim();
            if (clickedMenuItemLabel === 'Salir') {
                logout(); 
            }
        }
    };
</script>


<template>
    <div class="container-menu">
        <Menubar :model="items" @click="handleMenuItemClick">
            <template #start>
                <img alt="logo" src="../../assets/logo.png" height="40" class="logo" />
            </template>
        </Menubar>
    </div>
</template>

<style scoped>
    .container-menu {
        display: flex!important;
        justify-content: space-between!important;
        align-items: flex-end!important;
        background-color: white !important;
        width: 100%!important;
        height: 100%;
    }
    .p-menubar {
        border: none !important;
        background-color: white !important;
    }
    .p-menubar-start{
        display: inline!important;
        float: left!important;
        position: absolute!important;
    }

  

    .logo {
        height: 80px!important;
        width: 100%!important;
        margin-left: 10px!important;
        padding-right: 10px!important;
    }
  
</style>