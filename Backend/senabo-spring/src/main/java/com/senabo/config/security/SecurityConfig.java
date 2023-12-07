package com.senabo.config.security;


import com.senabo.config.security.jwt.JwtAuthenticationFilter;
import com.senabo.config.security.jwt.TokenProvider;
import lombok.RequiredArgsConstructor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityCustomizer;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;


@Configuration
@EnableWebSecurity
@RequiredArgsConstructor
public class SecurityConfig {
    private final TokenProvider tokenProvider;

    private static final String[] AUTH_WHITELIST = {
            "/member/sign-up",
            "/member/sign-in",
            "/member/reissue",
            "/swagger-ui.html",
            "/member/fcm",
            "/member/throw/token-check",
            "/member/throw/not-found",
            "/member/**",
            "/affection/**",
            "/bath/**",
            "/brushing-teeth/**",
            "/communication/**",
            "/disease/**",
            "/expense/**",
            "/feed/**",
            "/report/**",
            "/stress/**",
            "/walk/**",
            "/emergency/**"
    };

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity httpSecurity) throws Exception {
        httpSecurity.csrf(AbstractHttpConfigurer::disable);

        httpSecurity
                .cors(Customizer.withDefaults())
                .authorizeHttpRequests(authorize -> authorize
                        .requestMatchers(AUTH_WHITELIST).permitAll()
                        .anyRequest().authenticated()
                )
                .addFilterBefore(new JwtAuthenticationFilter(tokenProvider), UsernamePasswordAuthenticationFilter.class);

        return httpSecurity.build();
    }

    @Bean
    public WebSecurityCustomizer webSecurityCustomizer() {
        return (web) -> web.ignoring().requestMatchers("/v3/api-docs/**",   "/swagger-ui/**", "/api-docs/json/**");
    }
}
