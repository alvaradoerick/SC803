<template>
    <div class="card-container">
        <slot name="title">
            <h2 class="title">{{ $route.meta.title || 'ASEISTHMUS'}}</h2>
        </slot>
        <div class="card-body">
            <div class="card">
                <slot name="default"></slot>
            </div>
            
        </div>
        <footer class="footer">
            Hecho con ❤️ en Costa Rica
            <slot name="footer"></slot>
        </footer>
    </div>
</template>

<style scoped>

    .card-container {
        display: flex;
        min-height: 85vh;
        flex-direction: column;
        min-width:25rem
    }
    .card-body {
        flex: 1;
        margin-bottom: 3rem;
    }

    .title {
        font-size: 1.5rem;
        font-weight: bold;
        margin-bottom: 2rem;
        text-transform: uppercase;
        font-family: nunito;
        margin-top: 2rem;
    }
    .footer{
        margin-bottom: .1rem;
    }
</style>