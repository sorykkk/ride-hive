<!-- <script setup lang="ts">
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <div>
    <a href="https://vite.dev" target="_blank">
      <img src="/vite.svg" class="logo" alt="Vite logo" />
    </a>
    <a href="https://vuejs.org/" target="_blank">
      <img src="./assets/vue.svg" class="logo vue" alt="Vue logo" />
    </a>
  </div>
  <HelloWorld msg="Vite + Vue" />
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style> -->

<script setup>
import {ref, onMounted} from  'vue' // on Mounted -> cand codul din onMounted se executa, elementele vor fi disponibile

//Pentru a lua valori din backend
const counter = ref(0) // stocat pe frontend?

// async function fetchCounter(){
//   const res = await fetch('http://localhost:5030/Counter') //Get request pe Counter care l am facut
//   const data = await res.json()
//   counter.value = data.value //actualizeaza valoarea reactiva
// }

async function increment(){
  counter.value++;
  const res = await fetch(`http://localhost:5030/Counter?i=${counter.value}`, {method: 'POST'})
  const data = await res.json()
  console.log('Salvat în DB:', data)
}

// onMounted(fetchCounter)
</script>

<template>
    <div>
        <h1>
          Counter: {{counter }}
          <button @click = "increment">Increment</button>
        </h1>
    </div>
</template>