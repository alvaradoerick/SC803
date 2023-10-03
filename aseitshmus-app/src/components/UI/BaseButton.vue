"<script setup>
    import Button from "primevue/button"
    import { defineProps } from 'vue'


    defineProps({
        label: {
            type: String,
            required: true,
        },
        loading: {
            type: Boolean,
        },
        type: {
            type: String,
            required: true,
        },
        small: {
            type: Boolean,
            default: false,
        },
        color:{
            type: String,
            default: 'generic-style',
        },
    })

   
</script>

<template>
    <Button raised :loading="loading" :style="small ? 'width:8rem' : ''" :label=label :type=type :class="[color === 'green' ? 'green' : (color === 'red' ? 'red' : 'generic-style')]"></Button>
</template>

<style scoped>
    .generic-style {
        display: flex;
        background-color: #077CBC;
        border-color: #077CBC;
        overflow: hidden;
        width: 300px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .generic-style:hover,
    .generic-style:focus{
        box-shadow: 0 0 0 2px white, 0 0 0 3px skyblue;
        color: white;
        background-color: #0b4666 !important;
    }

    .green{
        display: flex;
        background-color: #3CA474;
        border-color: #3CA474;
        overflow: hidden;
        width: 300px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }
    .green:hover,
    .green:focus {
        box-shadow: 0 0 0 2px white, 0 0 0 3px #8fddb8!important;
        color: white;
        border-color: #21774f!important;
        background-color: #3a7b5d !important;
    }

    .red{
        display: flex;
        background-color: #873566;
        border-color: #a23677;
        overflow: hidden;
        width: 300px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }
    .red:hover,
    .red:focus {
        box-shadow: 0 0 0 2px white, 0 0 0 3px #a23677!important;
        color: white;
        border-color: #7a0d4f!important;
        background-color: #7a0d4f !important;
    }

</style>
